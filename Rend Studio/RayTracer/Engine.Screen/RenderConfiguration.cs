using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Plugins.Tracers;
using RayTracer.Plugins;
using RayTracer.Plugins.SpatialStructures;
using RayTracer.Plugins.Shaders;
using System.Drawing;

namespace RayTracer.Engine.Screen
{
    public class RenderConfiguration
    {
        public const float DefaultDpi = 96;
        public const int DefaultDepth = 5;
        public const int DefaultWidth = 640;
        public const int DefaultHeight = 480;

        public List<Action<Bitmap, Bitmap>> PostEffects { get; private set; }
        public float Dpi { get; set; }
        public float LightScale { get; set; }
        public bool AntiAliasing { get; set; }
        public int Depth { get; set; }
        public bool Animation { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Threads { get; set; }

        public bool DisableTextures { get; set; }
        public bool DisableEffects { get; set; }

        //Shaders
        public IShadowTracer ShadowTracer { get; set; }
        public ITextureFilter TextureFilter { get; set; }
        public ISuperSampler SuperSampler { get; set; }
        public ISpatialStructure SpatialStructure { get; set; }
        public List<IRecurseShader> RecurseShaders { get; private set; }
        public List<ILightShader> LightShaders { get; private set; }

        public RenderConfiguration()
        {
            RestoreDefaults();
        }

        public void RestoreDefaults()
        {
            LightScale = 1;
            PostEffects = new List<Action<Bitmap, Bitmap>>();
            Dpi = DefaultDpi;
            AntiAliasing = false;
            Depth = DefaultDepth;
            Animation = false;
            DisableEffects = false;
            DisableTextures = false;
            Height = DefaultHeight;
            Width = DefaultWidth;
            Threads = Environment.ProcessorCount;
            RecurseShaders = new List<IRecurseShader>();
            LightShaders = new List<ILightShader>();
            ShadowTracer = null;
            TextureFilter = null;
            SuperSampler = null;
            SpatialStructure = null;
        }
    }
}
