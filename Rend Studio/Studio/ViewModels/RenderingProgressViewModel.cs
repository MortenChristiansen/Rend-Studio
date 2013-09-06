using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Studio.ViewModels.Utils;

namespace Studio.ViewModels
{
    public class RenderingProgressViewModel : Screen
    {
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                if (_progress == value) return;
                _progress = value;
                NotifyOfPropertyChange(() => Progress);
            }
        }

        private RenderingProgressState _tracingRaysState;
        public RenderingProgressState TracingRaysState
        {
            get { return _tracingRaysState; }
            set
            {
                if (_tracingRaysState == value) return;
                _tracingRaysState = value;
                NotifyOfPropertyChange(() => TracingRaysState);
            }
        }

        private RenderingProgressState _loadingScriptingEngineState;
        public RenderingProgressState LoadingScriptingEngineState
        {
            get { return _loadingScriptingEngineState; }
            set
            {
                if (_loadingScriptingEngineState == value) return;
                _loadingScriptingEngineState = value;
                NotifyOfPropertyChange(() => LoadingScriptingEngineState);
            }
        }

        private RenderingProgressState _creatingSceneState;
        public RenderingProgressState CreatingSceneState
        {
            get { return _creatingSceneState; }
            set
            {
                if (_creatingSceneState == value) return;
                _creatingSceneState = value;
                NotifyOfPropertyChange(() => CreatingSceneState);
            }
        }

        public RenderingProgressViewModel()
        {
            DisplayName = "Rendering progress...";
        }
    }
}
