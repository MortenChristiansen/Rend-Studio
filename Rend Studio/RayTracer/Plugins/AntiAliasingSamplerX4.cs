using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;

namespace RayTracer.Plugins
{
    public class AntiAliasingSamplerX4 : AntiAliasingSamplerX2
    {
        public AntiAliasingSamplerX4()
        {
            _samples = 4;
        }

        #region IPlugin Members

        public new string Name
        {
            get { return "Anti Aliasing x4"; }
        }

        #endregion
    }
}
