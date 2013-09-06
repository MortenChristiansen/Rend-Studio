using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Autofac;

namespace Studio.Infrastructure.Caliburn
{
    public class ViewModel : Screen
    {
        public ViewModel()
        {
            IoC.BuildUp(this);
        }
    }
}
