using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Caliburn.Micro;
using Microsoft.Scripting;
using Microsoft.Win32;
using RayTracer.Engine;
using Studio.Assets.Project;
using Studio.Infrastructure.Caliburn;
using Studio.RenderingPipeline;
using Studio.ViewModels.Utils;
using WpfShell.Controls;

namespace Studio.ViewModels
{
    public class SceneViewModel : ViewModel
    {
        private string _code = "";
        private SyntaxErrorException _syntaxError;
        private bool _isSaved;
        private bool _isScriptModeEnabled;
        private RenderingViewModel _currentRendering;
        private bool _ignoreFirstAttemptToLeaveCodeMode;

        // Dependencies
        public IWindowManager WindowManager { get; set; }

        public RenderingViewModel CurrentRendering
        {
            get
            {
                return _currentRendering;
            }
            set
            {
                if (_currentRendering == value) return;
                _currentRendering = value;
                NotifyOfPropertyChange(() => CurrentRendering);
                IsScriptModeActive = false;
            }
        }
        public string Code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    IsSaved = false;
                    _code = value;
                    NotifyOfPropertyChange(() => Code);
                }
            }
        }
        public SyntaxErrorException SyntaxError
        {
            get { return _syntaxError; }
            set
            {
                if (_syntaxError != value)
                {
                    _syntaxError = value;
                    NotifyOfPropertyChange(() => SyntaxError);
                }
            }
        }
        public string Title { get; set; }
        public ObservableCollection<RenderingViewModel> Renderings { get; private set; }
        public bool IsSaved
        {
            get { return _isSaved; }
            set
            {
                _isSaved = value;

                DisplayName = Title;
                if (!_isSaved)
                    DisplayName += "*";
            }
        }
        public bool IsScriptModeActive
        {
            get { return _isScriptModeEnabled; }
            set
            {
                // This check prevents the scene to start in the renderings gallery
                // when being serialized with existing renderings.
                if (_ignoreFirstAttemptToLeaveCodeMode && value == false)
                {
                    _ignoreFirstAttemptToLeaveCodeMode = false;
                    return;
                }
                if (_isScriptModeEnabled == value) return;
                _isScriptModeEnabled = value;
                NotifyOfPropertyChange(() => IsScriptModeActive);
            }
        }
        public ImageSource Icon
        {
            get { return BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Icons/clapperboard.png", UriKind.RelativeOrAbsolute)); }
        }
        public SceneAsset SceneAsset { get; private set; }

        /// <summary>
        /// Creates a new scene model with a default title.
        /// </summary>
        public SceneViewModel()
        {
            SceneAsset = new SceneAsset();
            Initialize(SceneAsset);
        }

        /// <summary>
        /// Create a scene from a scene asset.
        /// </summary>
        /// <param name="source">The asset</param>
        public SceneViewModel(SceneAsset asset)
        {
            SceneAsset = asset;
            Initialize(SceneAsset);
        }

        private void Initialize(SceneAsset asset)
        {
            Title = asset.Title;
            Code = asset.Code;
            IsSaved = true;
            Renderings = new ObservableCollection<RenderingViewModel>();

            _ignoreFirstAttemptToLeaveCodeMode = asset.Renderings.Any();

            foreach (var renderingAsset in asset.Renderings)
                Renderings.Add(new RenderingViewModel(renderingAsset));

            IsScriptModeActive = true;
        }

        public override void CanClose(Action<bool> callback)
        {
            if (IsSaved)
            {
                callback(true);
                return;
            }

            var result = WpfMessageBox.Show("The scene '{0}' contains unsaved data. Do you want to save the file before closing?".Args(Title), "Unsaved data", MessageBoxButton.YesNoCancel);
            
            switch (result)
	        {
		        case MessageBoxResult.Cancel:
                    callback(false);
                    break;
                case MessageBoxResult.No:
                    callback(true);
                    break;
                case MessageBoxResult.Yes:
                    Save();
                    callback(IsSaved);
                    break;
	        }
        }

        public void Save()
        {
            SaveAsset(Persistence.SaveAsset);
        }

        public void SaveAs()
        {
            SaveAsset(Persistence.SaveAssetAs);
        }

        private void SaveAsset(Func<Asset, bool> save)
        {
            SceneAsset.Code = Code;

            var saved = save(SceneAsset);
            if (saved)
            {
                Title = SceneAsset.Title;
                IsSaved = true;
            }
        }

        public void ShowEditor()
        {
            IsScriptModeActive = true;
        }

        public void ShowRenderings()
        {
            IsScriptModeActive = false;
        }

        public void Render()
        {
            Save();
            if (!IsSaved)
                return;

            var context = TaskScheduler.FromCurrentSynchronizationContext();

            var progress = new RenderingProgressViewModel();
            Bitmap rendering = null;
            Exception error = null;

            Task.Factory.StartNew(() =>
            {
                // Render the image on a worker thread
                progress.LoadingScriptingEngineState = RenderingProgressState.Started;
                
                SyntaxError = null;
                ScriptingSystem scriptParser = scriptParser = new ScriptingSystem();
                progress.LoadingScriptingEngineState = RenderingProgressState.Completed;
                progress.Progress = 33;
                progress.CreatingSceneState = RenderingProgressState.Started;
                var scene = scriptParser.CreateScene(Code);

                progress.Progress = 66;
                if (scriptParser.HasError)
                {
                    progress.CreatingSceneState = RenderingProgressState.Failed;
                    progress.TracingRaysState = RenderingProgressState.Aborted;
                    error = scriptParser.Error;
                    HandleScriptError(scriptParser.Error);
                }
                else
                {
                    progress.CreatingSceneState = RenderingProgressState.Completed;
                    progress.TracingRaysState = RenderingProgressState.Started;
                    rendering = RenderScene(scene);
                    progress.TracingRaysState = RenderingProgressState.Completed;
                }
                progress.Progress = 100;
            }).ContinueWith(t =>
            {
                // Update the UI on the UI thread
                if (rendering != null)
                {
                    var imageAsset = new RenderingAsset(rendering);
                    SceneAsset.AddRendering(imageAsset);
                    Persistence.SaveAsset(imageAsset);
                    CurrentRendering = new RenderingViewModel(imageAsset);

                }
            }, context).ContinueWith(t => 
            {
                // Wait a moment on the worker thread before closing the window
                Thread.Sleep(400);
            }).ContinueWith(t =>
            {
                // Close the window on the UI thread
                progress.TryClose();
                Renderings.Add(CurrentRendering);
            }, context);

            WindowManager.ShowDialog(progress);

            if (error == null)
            {
                Save();
            }
            else
            {
                if (SyntaxError != null)
                {
                    WpfMessageBox.Show("The script contained a syntax error: " + error.Message + ".", "Syntax error");
                }
                else
                {
                    WpfMessageBox.Show("There was an error executing the script: " + error.Message + ".", "Error");
                }
            }
        }

        private Bitmap RenderScene(Scene scene)
        {
            var renderer = new Renderer();
            return renderer.Render(scene);
        }

        private void HandleScriptError(Exception e)
        {
            var syntaxError = e as SyntaxErrorException;
            if (syntaxError != null)
                SyntaxError = syntaxError;
        }
    }
}
