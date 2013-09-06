namespace Studio {
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;
    using Microsoft.Win32;
    using Studio.Assets.Project;
    using Studio.ViewModels;

    public class ShellViewModel : IShell, INotifyPropertyChanged
    {
        private ImageSource _icon;

        public string Title { get; private set; }

        public SceneEditorViewModel SceneEditor { get; private set; }
        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    Uri iconUri = new Uri("pack://application:,,,/Resources/brand.ico", UriKind.RelativeOrAbsolute);
                    _icon = BitmapFrame.Create(iconUri);
                }
                return _icon;
            }
        }

        public ProjectViewModel Project { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ShellViewModel()
        {
            SceneEditor = new SceneEditorViewModel { Parent = this };

            Title = "Rend Studio";
        }

        private void ConfigureMenu()
        {

        }

        public void AddNewScene()
        {
            SceneEditor.OpenTab();
        }

        public void OpenScene()
        {
            var sceneAsset = Persistence.OpenAsset<SceneAsset>();
            if (sceneAsset != null)
                SceneEditor.OpenTab(new SceneViewModel(sceneAsset));
        }

        public void CreateNewProject()
        {
            Project = new ProjectViewModel(SceneEditor) { Parent = this };
            PropertyChanged(this, new PropertyChangedEventArgs("Project"));
        }

        public void OpenProject()
        {
            var projectAsset = Persistence.OpenAsset<ProjectAsset>();
            if (projectAsset != null)
            {
                Project = new ProjectViewModel(projectAsset, SceneEditor) { Parent = this };
                PropertyChanged(this, new PropertyChangedEventArgs("Project"));
            }
        }
    }
}
