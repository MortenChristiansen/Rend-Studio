using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RayTracer.Engine;
using RayTracer.Geometry;
using RayTracer.Geometry.Coloring;
using RayTracer.Geometry.Lighting;
using RayTracer.Geometry.Primitives;
using RayTracer.Mathematics;
using RayTracer.Plugins;
using RayTracer.Plugins.Models;
using RayTracer.Plugins.Shaders;
using RayTracer.Plugins.SpatialStructures;
using RayTracer.Plugins.Tracers;
using WpfShell.Controls;
using Color = RayTracer.Geometry.Coloring.Color;

namespace Studio.RenderingPipeline
{
    public class Renderer
    {
        public Bitmap Render(Scene scene)
        {
            ConfigureRenderingEngine();

            RenderEngine.Initialize(scene);
            var spatialTime = RenderEngine.PopulateSpatialStructure();
            var frame = RenderEngine.RenderScene();
            
            var formatted =
@"Rays per second: {0:#,##0.0}
Render time: {1:0.00}s
Spation structure creation time: {2:0.00}s
Primitives: {3}".Args(frame.RaysPerSecond, frame.RenderTime.TotalSeconds, spatialTime.TotalSeconds, RenderEngine.Scene.Primitives.Count);

            formatted += "\nNode count: {0}".Args(frame.RenderingInfo["node count"]);
            formatted += "\nRay traces: {0:#,##0}".Args(frame.RenderingInfo["ray traces"]);
            formatted += "\nRays/pixel: {0:0.0}".Args(frame.RenderingInfo["rays per pixel"]);
            formatted += "\nTime/pixel: {0:0.0000}ms".Args(frame.RenderingInfo["time per pixel"]);
            //foreach (var info in frame.RenderingInfo)
            //{
            //    formatted += "\n{0}: {1}".Args(info.Key, info.Value);
            //}
            //var traces = "{0:#,##0}".Args(_renderUnits.Sum(u => u.Traces));
            //var rps = "{0:0}".Args((float)_renderUnits.Sum(u => u.Traces) / (_pixels.Length * _pixels[0].Length));
            //var tpp = "{0:0.000000}".Args((endRenderingTime - startRenderingTime).TotalSeconds / (_pixels.Length * _pixels[0].Length));

            System.Windows.MessageBox.Show(formatted, "Rendering Metrics");

            return frame.Image;
        }

        private static void ConfigureRenderingEngine()
        {
            RenderEngine.Configuration.RestoreDefaults();
            RenderEngine.Configuration.SpatialStructure = new KdTree();
            RenderEngine.Configuration.AntiAliasing = true;
            //RenderEngine.Configuration.SuperSampler = new AntiAliasingSamplerX8();
            RenderEngine.Configuration.RecurseShaders.Add(new ReflectionColorShader());
            RenderEngine.Configuration.RecurseShaders.Add(new GlossyReflectionShader());
            RenderEngine.Configuration.RecurseShaders.Add(new RefractionColorShader());
            RenderEngine.Configuration.LightShaders.Add(new SpecularColorShader());
            RenderEngine.Configuration.LightShaders.Add(new DiffuseColorShader());
            RenderEngine.Configuration.ShadowTracer = new HardShadowTracer();
        }

        private void LoadScene()
        {
            var steel = new Material(0.3f, 10f, 0.4f, 1f, 0f, new Color(160, 160, 160));
            var wood = new Material(0.1f, 10f, 0.2f, 1f, 0f, new Color(150, 87, 48));
            var podium1 = new Box(new Vector(-200, 0, 0), new Vector(80, 130, 80), wood);
            var ball1 = new Sphere(new Vector(-160, 160, 40), 30, steel);

            var light1 = new PointLight(300000f, new Vector(-180, 200, -200), Color.White);
            var light2 = new PointLight(400000f, new Vector(180, 200, -200), Color.White);
            var ambientLight = new AmbientLight(0.2f, Color.White);

            RenderEngine.Scene.AddPrimitive(podium1);
            RenderEngine.Scene.AddPrimitive(ball1);
            RenderEngine.Scene.AddPrimitive(light1);
            RenderEngine.Scene.AddPrimitive(light2);
            RenderEngine.Scene.AmbientLights.Add(ambientLight);

            RenderEngine.Scene.Camera.Position = new Vector(-100, 100, -800);
            RenderEngine.Scene.Camera.Direction = new Vector(100, 50, 800);
        }

        public static void LoadMixedScene2()
        {
            var glass = new Material(0.6f, 10f, 0.5f, 1.33f, 1f, Color.White);
            var wood = new Material(0.1f, 10f, 0.2f, 1f, 0f, new Color(150, 87, 48));
            var steel = new Material(0.3f, 10f, 0.4f, 1f, 0f, new Color(160, 160, 160));

            var podium1 = new Box(new Vector(-200, 0, 0), new Vector(80, 130, 80), steel);
            var podium2 = new Box(new Vector(-40, 0, 0), new Vector(80, 180, 80), steel);
            var podium3 = new Box(new Vector(120, 0, 0), new Vector(80, 130, 80), steel);

            var ball1 = new Sphere(new Vector(-160, 160, 40), 30, glass);
            var ball2 = new Sphere(new Vector(0, 210, 40), 30, glass);
            var ball3 = new Sphere(new Vector(150, 160, 40), 30, glass);

            var floor = new Box(new Vector(-1000, -0.5f, -1000), new Vector(2000, 0.5f, 2000), wood);
            var centerWall = new Box(new Vector(-1000, 0, 150), new Vector(2000, 2000, 0.5f), wood);
            var rightWall = new Box(new Vector(220, 0, -1000), new Vector(0.5f, 2000, 2000), wood);

            var light1 = new PointLight(30000f, new Vector(-180, 200, -200), Color.White);
            var light2 = new PointLight(40000f, new Vector(180, 200, -200), Color.White);
            var ambientLight = new AmbientLight(0.2f, Color.White);

            RenderEngine.Scene.AddPrimitive(podium1);
            RenderEngine.Scene.AddPrimitive(podium2);
            RenderEngine.Scene.AddPrimitive(podium3);
            RenderEngine.Scene.AddPrimitive(ball1);
            RenderEngine.Scene.AddPrimitive(ball2);
            RenderEngine.Scene.AddPrimitive(ball3);
            RenderEngine.Scene.AddPrimitive(floor);
            RenderEngine.Scene.AddPrimitive(centerWall);
            RenderEngine.Scene.AddPrimitive(rightWall);
            RenderEngine.Scene.AddPrimitive(light1);
            RenderEngine.Scene.AddPrimitive(light2);
            //RenderEngine.Scene.AmbientLights.Add(ambientLight);

            RenderEngine.Scene.Camera.Position = new Vector(-100, 100, -800);
            RenderEngine.Scene.Camera.Direction = new Vector(100, 50, 800);
            //RenderEngine.Scene.Camera.Zoom = 800.0d;
        }

        public static void LoadSimpleScene()
        {
            float scale = 0.1f;
            var material = new Material(0.1f, 10f, 0.2f, 1f, 0f, new Color(150, 87, 48));
            //var box = new Box(new Vector(-1000, -1000, 0), new Vector(2000, 2000, 100), material);
            var triangle1 = new Triangle(new Vector(-1000, -1000, 0) * scale, new Vector(-1000, 1000, 0) * scale, new Vector(1000, 1000, 0) * scale) { Material = material };
            var triangle2 = new Triangle(new Vector(-1000, -1000, 0) * scale, new Vector(1000, 1000, 0) * scale, new Vector(1000, -1000, 0) * scale) { Material = material };
            var light = new PointLight(4000f, new Vector(0, 0, -900) * scale, Color.White);
            var light2 = new PointLight(4000f, new Vector(200, 0, -900) * scale, Color.White);
            RenderEngine.Scene.AddPrimitive(triangle1);
            RenderEngine.Scene.AddPrimitive(triangle2);
            RenderEngine.Scene.AddPrimitive(light);
            RenderEngine.Scene.AddPrimitive(light2);

            var lightSphere = new Sphere(new Vector(0, 0, -900) * scale, 10, new Material(0, 0, 0, 1, 0.9f, Color.White));
            RenderEngine.Scene.AddPrimitive(lightSphere);

            RenderEngine.Scene.Camera.Position = new Vector(50, 50, -400);
            //RenderEngine.Scene.Camera.Zoom = 500;
            RenderEngine.Scene.Camera.Direction = RenderEngine.Scene.Camera.Position.Invert();
        }

        public static void LoadSparseScene()
        {
            Scene scene = RenderEngine.Scene;

            Material mat1 = new Material(0.1f, 0.1f, 0.5f, 1.02f, 0.6f, Colors.Aqua);
            Sphere sphere1 = new Sphere(new Vector(-40, -90, 290), 5, mat1);
            Sphere sphere2 = new Sphere(new Vector(-40, -70, 290), 5, mat1);
            Sphere sphere3 = new Sphere(new Vector(-40, -50, 290), 5, mat1);
            Sphere sphere4 = new Sphere(new Vector(-40, -30, 290), 5, mat1);
            Sphere sphere5 = new Sphere(new Vector(-40, -10, 290), 5, mat1);
            Sphere sphere6 = new Sphere(new Vector(240, -50, 290), 5, mat1);
            scene.AddPrimitive(sphere1);
            scene.AddPrimitive(sphere2);
            scene.AddPrimitive(sphere3);
            scene.AddPrimitive(sphere4);
            scene.AddPrimitive(sphere5);
            scene.AddPrimitive(sphere6);
            PointLight light = new PointLight(1.0f, new Vector(30, -40, 100), Colors.White);
            scene.AddPrimitive(light);
            Material mat5 = new Material(0.2f, 0.7f, 0.07f, 1.0f, 0.0f, Colors.Tomato);
            Box box1 = new Box(new Vector(240, -150, 260), new Vector(50, 40, 60), mat5);
            scene.AddPrimitive(box1);
        }
    }
}
