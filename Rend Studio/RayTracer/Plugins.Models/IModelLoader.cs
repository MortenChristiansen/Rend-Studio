using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Models;

namespace RayTracer.Plugins.Models
{
    public interface IModelLoader : IPlugin
    {
        Model ParseFile(string filePath);
    }
}
