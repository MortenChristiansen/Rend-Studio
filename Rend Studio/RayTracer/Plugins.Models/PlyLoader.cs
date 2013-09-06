using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Models;

namespace RayTracer.Plugins.Models
{
    public class PlyLoader : IModelLoader
    {
        #region IModelLoader Members

        //filepath could be a conf file
        public Model ParseFile(string filePath)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPlugin Members

        public string Name
        {
            get { return "Ply Model Loader"; }
        }

        #endregion
    }
}
