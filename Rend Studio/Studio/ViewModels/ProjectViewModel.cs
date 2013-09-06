using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Studio.Assets.Project;
using Studio.Core.BasicHelpers;
using Studio.Core.Mvvm;
using Studio.Infrastructure.Caliburn;
using Studio.ViewModels.Utils;

namespace Studio.ViewModels
{
    public class ProjectViewModel : ViewModel
    {
        private ProjectAsset _projectAsset;
        private SceneEditorViewModel _sceneEditor;

        public SortableObservableCollection<EditableViewModel<SceneViewModel>> Scenes { get; private set; }

        public ProjectViewModel(ProjectAsset projectAsset, SceneEditorViewModel sceneEditor)
        {
            _sceneEditor = sceneEditor;
            Scenes = new SortableObservableCollection<EditableViewModel<SceneViewModel>>();

            _projectAsset = projectAsset ?? new ProjectAsset();
            Persistence.SaveAsset(_projectAsset);
            DisplayName = _projectAsset.Title;

            LoadAsset();
        }

        private void LoadAsset()
        {
            foreach (var scene in _projectAsset.Scenes)
            {
                Scenes.Add(new EditableViewModel<SceneViewModel>(new SceneViewModel(scene)));
            }
        }

        public ProjectViewModel(SceneEditorViewModel sceneEditor)
            : this(null, sceneEditor)
        {
            
        }

        public void AddNewScene()
        {
            var scene = new SceneViewModel();
            _projectAsset.AddScene(scene.SceneAsset);
            Persistence.SaveAsset(scene.SceneAsset);
            Persistence.SaveAsset(_projectAsset);

            Scenes.Add(new EditableViewModel<SceneViewModel>(scene));
            Scenes.Sort(s => s.ViewModel.DisplayName);

            _sceneEditor.OpenTab(scene);
        }

        
        public void OpenOrActivate(SceneViewModel scene)
        {
            // Checks for double click
            if (ThrottleHelpers.HasBeenCalledWithin(TimeSpan.FromMilliseconds(300)))
            {
                _sceneEditor.ActivateItem(scene); // This should also autofocus the code editor
            }
        }

        public void Rename()
        {
            Scenes.First().IsEditing = true;
        }

        public bool CanRename()
        {
            return true;
        }
    }
}

// Show Projects menu when a project is open