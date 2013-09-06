using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Studio.ViewModels
{
    public class SceneEditorViewModel : Conductor<SceneViewModel>.Collection.OneActive
    {
        public void OpenTab()
        {
            ActivateItem(new SceneViewModel());
        }

        public void OpenTab(SceneViewModel scene)
        {
            ActivateItem(scene);
        }

        public void CloseItem(SceneViewModel scene)
        {
            this.CloseItem((object)scene);
        }

        public void SaveCurrent()
        {
            ActiveItem.Save();
        }

        public bool CanSaveCurrent()
        {
            return !ActiveItem.IsSaved;
        }

        public void SaveCurrentAs()
        {
            ActiveItem.SaveAs();
        }

        public void SaveAll()
        {
            foreach (var item in Items)
            {
                item.Save();
            }
        }

        public bool CanSaveAll()
        {
            return Items.Any(i => !i.IsSaved);
        }

        public void RenderCurrent()
        {
            ActiveItem.Render();
        }

        public bool CanRenderCurrent()
        {
            return ActiveItem != null;
        }
    }
}
