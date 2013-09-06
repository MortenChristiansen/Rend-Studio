using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using RayTracer.Engine.Screen;
using RayTracer.Geometry;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Plugins.SpatialStructures;

namespace RayTracer.Engine
{
    public class RenderEngine
    {
        private static Color[][] _pixels;
        private static List<Thread> _threadPool;
        private static int[] _renderedPixels;
        private static RenderUnit[] _renderUnits;
        private static int _workCounter;
        private static int _totalPixels;

        public static RenderConfiguration Configuration { get; private set; }
        public static Scene Scene { get; private set; }
        public static int RenderingPercentage { get; private set; }
        public static ScriptingSystem ScriptingSystem { get; private set; }
        public static TextWriter Out { get; private set; }
        public static int ExecutingThreadId { get { return _threadPool.IndexOf(Thread.CurrentThread); } }

        internal static RenderUnit RenderUnit
            { get { return ExecutingThreadId < 0 ? new RenderUnit() : _renderUnits[ExecutingThreadId]; } }

        public static event Action<int> RenderingProgress;


        static RenderEngine()
        {
            Configuration = new RenderConfiguration();
        }

        /// <summary>
        /// Initializes the engine to use an existing scene.
        /// </summary>
        /// <param name="scene">The scene to render</param>
        public static void Initialize(Scene scene)
        {
            Scene = scene;
            RenderingPercentage = 0;
            ScriptingSystem = new ScriptingSystem();
            Out = Console.Out;

            //Precalculations
            _renderUnits = new RenderUnit[Configuration.Threads];
            for (int i = 0; i < _renderUnits.Length; i++)
            {
                _renderUnits[i] = new RenderUnit();
            }
        }

        /// <summary>
        /// Initializes the engine with a new the scene;
        /// </summary>
        public static void Initialize()
        {
            Initialize(new Scene());
        }

        public static TimeSpan PopulateSpatialStructure()
        {
            DateTime startSpatialStructuring = DateTime.UtcNow;
            if (Configuration.SpatialStructure != null)
            {
                Configuration.SpatialStructure = Configuration.SpatialStructure.GetStructureInstance(Scene.Primitives.ToArray());
                Configuration.SpatialStructure.Initialize();
            }
            return DateTime.UtcNow - startSpatialStructuring;
        }

        public static Frame RenderScene()
        {
            _totalPixels = Scene.Camera.Height * Scene.Camera.Width;
            _renderedPixels = Enumerable.Repeat(0, Configuration.Threads).ToArray();
            InitializeThreadPool();
            _workCounter = Scene.Camera.Width;
            _pixels = new Color[Scene.Camera.Width][];

            int nodeCount = 0;
            if (Configuration.SpatialStructure is KdTree)
            {
                nodeCount = ((KdTree)Configuration.SpatialStructure).NodeCount;
            }

            DateTime startRenderingTime = DateTime.UtcNow;

            for (int i = 0; i < Configuration.Threads; i++)
            {
                _threadPool[i].Start(i);
            }
            for (int i = 0; i < Configuration.Threads; i++)
            {
                _threadPool[i].Join();
            }

            DateTime endRenderingTime = DateTime.UtcNow;

            Dictionary<string, object> debugInfo = new Dictionary<string, object>() { { "node count", nodeCount }, { "ray traces", _renderUnits.Sum(u => u.Traces) }, { "rays per pixel", (float)_renderUnits.Sum(u => u.Traces) / (_pixels.Length * _pixels[0].Length) }, { "time per pixel", (endRenderingTime - startRenderingTime).TotalMilliseconds / (_pixels.Length * _pixels[0].Length) } };
            return new Frame(_pixels, endRenderingTime - startRenderingTime, debugInfo);
        }

        private static void RenderPixels(object threadId)
        {
            int thread = (int)threadId;
            Camera camera = Scene.Camera;

            Thread.BeginThreadAffinity();

            #region Work Stealing

            //int workId = Interlocked.Decrement(ref _workCounter);

            //while (workId >= 0)
            //{
            //    Ray[] rays = new Ray[camera.Height];
            //    for (int rayId = 0; rayId < rays.Length; rayId++)
            //    {
            //        //Anti aliasing?
            //        //Camera mode?
            //        Vector direction = camera.GetFrameCoordinateDirection(workId, rayId).Normal;
            //        rays[rayId] = new Ray(camera.Position, direction);
            //    }

            //    Color[] line = SuperSampler == null ? _renderUnits[thread].RenderRays(rays) : SuperSampler.RenderRays(rays);
            //    _pixels[workId] = line;

            //    //Update rendering stats
            //    _renderedPixels[thread] += camera.Height;
            //    int oldProgress = RenderingPercentage;
            //    RenderingPercentage = (_renderedPixels.Sum() * 100) / _totalPixels;
            //    if (RenderingPercentage > oldProgress && RenderingProgress != null) RenderingProgress(RenderingPercentage);

            //    workId = Interlocked.Decrement(ref _workCounter);
            //}

            #endregion

            for (int lineNo = thread; lineNo < camera.Width; lineNo += Configuration.Threads)
            {
                Ray[] rays = new Ray[camera.Height];
                for (int rayId = 0; rayId < rays.Length; rayId++)
                {
                    //Anti aliasing?
                    //Camera mode?
                    Vector direction = camera.GetFrameCoordinateDirection(lineNo, rayId).Normal;
                    rays[rayId] = new Ray(camera.Position, direction);
                }

                Color[] line = Configuration.SuperSampler == null ? _renderUnits[thread].RenderRays(rays) : Configuration.SuperSampler.RenderRays(rays);
                _pixels[lineNo] = line;

                //Update rendering stats
                _renderedPixels[thread] += camera.Height;
                int oldProgress = RenderingPercentage;
                RenderingPercentage = (_renderedPixels.Sum() * 100) / _totalPixels;
                if (RenderingPercentage > oldProgress && RenderingProgress != null) RenderingProgress(RenderingPercentage);
            }

            Thread.EndThreadAffinity();
        }

        private static void InitializeThreadPool()
        {
            _threadPool = new List<Thread>();
            for (int i = 0; i < Configuration.Threads; i++)
            {
                var thread = new Thread(new ParameterizedThreadStart(RenderPixels));
                _threadPool.Add(thread);
            }
        }

        #region Renderfunction Compilation

        //private delegate Color ShaderExecution(Vector intersection, int depth, Color colorAcc, Primitive hitPrimitive, Ray ray, RayCollision collisionResult, float refractionIndex);
        //private static ShaderExecution PrepareShaderFunction(PositionedLight light1, PositionedLight light2, ILightShader lightShader1, ILightShader lightShader2, IRecurseShader recurseShader1, IRecurseShader recurseShader2, Color ambientColor, float ambientBrightness)
        //{
        //    return (intersection, depth, colorAcc, hitPrimitive, ray, collisionResult, refractionIndex) =>
        //        {
        //            float shade1 = ShadowTracer == null ? 1.0f : ShadowTracer.CalculateShade(intersection, light1, depth);
        //            lightShader1.AdjustColor(ref colorAcc, hitPrimitive, light1, intersection, ray.Direction, shade1);
        //            lightShader2.AdjustColor(ref colorAcc, hitPrimitive, light1, intersection, ray.Direction, shade1);

        //            float shade2 = ShadowTracer == null ? 1.0f : ShadowTracer.CalculateShade(intersection, light2, depth);
        //            lightShader1.AdjustColor(ref colorAcc, hitPrimitive, light2, intersection, ray.Direction, shade2);
        //            lightShader2.AdjustColor(ref colorAcc, hitPrimitive, light2, intersection, ray.Direction, shade2);

        //            recurseShader1.AdjustColor(ref colorAcc, intersection, hitPrimitive, ray, depth, refractionIndex, collisionResult);
        //            recurseShader2.AdjustColor(ref colorAcc, intersection, hitPrimitive, ray, depth, refractionIndex, collisionResult);

        //            return Color.Add(colorAcc, Color.Multiply(ambientColor, ambientBrightness));
        //        };
        //}

        //private static PositionedLight _light1;
        //private static PositionedLight _light2;
        //private static ILightShader _lightShader1;
        //private static ILightShader _lightShader2;
        //private static IRecurseShader _recurseShader1;
        //private static IRecurseShader _recurseShader2;
        //private static Color _ambientColor;
        //private static float _ambientBrightness;

        //private static void PrepareShaders(PositionedLight light1, PositionedLight light2, ILightShader lightShader1, ILightShader lightShader2, IRecurseShader recurseShader1, IRecurseShader recurseShader2, Color ambientColor, float ambientBrightness)
        //{
        //    _light1 = light1;
        //    _light2 = light2;
        //    _lightShader1 = lightShader1;
        //    _lightShader2 = lightShader2;
        //    _recurseShader1 = recurseShader1;
        //    _recurseShader2 = recurseShader2;
        //    _ambientColor = ambientColor;
        //    _ambientBrightness = ambientBrightness;
        //}

        //private static void ExecuteShaders(Vector intersection, int depth, ref Color colorAcc, Primitive hitPrimitive, Ray ray, RayCollision collisionResult, float refractionIndex)
        //{
        //    float shade1 = ShadowTracer == null ? 1.0f : ShadowTracer.CalculateShade(intersection, _light1, depth);
        //    _lightShader1.AdjustColor(ref colorAcc, hitPrimitive, _light1, intersection, ray.Direction, shade1);
        //    _lightShader2.AdjustColor(ref colorAcc, hitPrimitive, _light1, intersection, ray.Direction, shade1);

        //    float shade2 = ShadowTracer == null ? 1.0f : ShadowTracer.CalculateShade(intersection, _light2, depth);
        //    _lightShader1.AdjustColor(ref colorAcc, hitPrimitive, _light2, intersection, ray.Direction, shade2);
        //    _lightShader2.AdjustColor(ref colorAcc, hitPrimitive, _light2, intersection, ray.Direction, shade2);

        //    _recurseShader1.AdjustColor(ref colorAcc, intersection, hitPrimitive, ray, depth, refractionIndex, collisionResult);
        //    _recurseShader2.AdjustColor(ref colorAcc, intersection, hitPrimitive, ray, depth, refractionIndex, collisionResult);

        //    colorAcc = Color.Add(colorAcc, Color.Multiply(_ambientColor, _ambientBrightness).Filter(colorAcc));
        //}

        #endregion
    }
}
