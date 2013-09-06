using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Studio.ViewModels.Utils
{
    public enum RenderingProgressState
    {
        NotStarted,
        Started,
        Completed,
        Failed,
        Aborted
    }
}
