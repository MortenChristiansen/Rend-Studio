using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Plugins
{
    public class AntiAliasingSamplerX3 : AntiAliasingSamplerX2
    {
        public AntiAliasingSamplerX3()
        {
            _samples = 3;
        }

        #region IPlugin Members

        public new string Name
        {
            get { return "Anti Aliasing x3"; }
        }

        #endregion
    }
}
