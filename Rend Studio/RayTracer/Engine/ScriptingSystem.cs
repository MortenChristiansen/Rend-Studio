using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using Microsoft.Scripting;

namespace RayTracer.Engine
{
    public class ScriptingSystem
    {
        private static ScriptEngine _engine;

        public bool HasError { get; private set; }
        public Exception Error { get; private set; }
        public static bool IsInitialized { get { return _engine != null; } }

        public ScriptingSystem()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (!IsInitialized)
            {
                _engine = Python.CreateEngine();
                _engine.Runtime.LoadAssembly(typeof(string).Assembly);
                _engine.Runtime.LoadAssembly(typeof(RenderEngine).Assembly);
            }
            
        }

        public Scene CreateScene(string script)
        {
            HasError = false;
            Error = null;

            var scope = _engine.CreateScope();

            var scene = new Scene();
            scope.SetVariable("Scene", scene);
            scope.SetVariable("Config", RenderEngine.Configuration);
            scope.ImportModule("clr");

            _engine.Execute(@"
clr.AddReference(""RayTracer"")
from RayTracer.Geometry.Coloring import *
from RayTracer.Geometry.Primitives import *
from RayTracer.Geometry.Lighting import *
from RayTracer.Mathematics import *", scope);

            try
            {
                _engine.Execute(script, scope);
            }
            catch (Exception e)
            {
                HasError = true;
                Error = e;
            }


            return scene;
        }
    }
}
