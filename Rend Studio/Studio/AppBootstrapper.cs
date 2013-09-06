using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using Caliburn.Micro.Logging;
using Studio.Infrastructure.Controls;

namespace Studio
{
    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        public AppBootstrapper()
        {
            LogManager.GetLog = type => new DebugLogger(type);
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();

            EnforceNamespaceConvention = false;
            ViewModelBaseType = typeof(IShell);
        }
    }
}
