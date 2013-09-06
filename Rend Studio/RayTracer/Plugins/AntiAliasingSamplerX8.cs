using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Plugins
{
    public class AntiAliasingSamplerX8 : AntiAliasingSamplerX2
    {
        public AntiAliasingSamplerX8()
        {
            _samples = 8;
        }

        #region IPlugin Members

        public new string Name
        {
            get { return "Anti Aliasing x8"; }
        }

        #endregion
    }
}
