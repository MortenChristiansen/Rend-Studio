using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Color = RayTracer.Geometry.Coloring.Color;
using System.Drawing;
using RayTracer.Plugins;
using System.Diagnostics;
using RayTracer.PostProcessing;

namespace RayTracer.Engine
{
    public class Frame
    {
        public Color[][] Pixels { get; private set; }
        public TimeSpan RenderTime { get; private set; }
        public TimeSpan PostProcessingTime { get; private set; }
        public float RaysPerSecond { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public Dictionary<string, object> RenderingInfo { get; private set; }
        private Bitmap _image;
        public Bitmap Image
        {
            get
            {
                if (_image == null) DrawBitmaps();
                return _image;
            }
        }
        private Bitmap _overlight;
        public Bitmap Overlight
        {
            get
            {
                if (_overlight == null) DrawBitmaps();
                return _overlight;
            }
        }

        internal Frame(Color[][] pixels, TimeSpan renderTime)
            : this(pixels, renderTime, null)
        {
            
        }

        internal Frame(Color[][] pixels, TimeSpan renderTime, Dictionary<string, object> renderingInfo)
        {
            Pixels = pixels;
            Width = RenderEngine.Scene.Camera.Width;
            Height = RenderEngine.Scene.Camera.Height;
            RenderTime = renderTime;
            RaysPerSecond = (int)renderingInfo["ray traces"] / (float)renderTime.TotalSeconds;
            RenderingInfo = renderingInfo;
            PostProcessingTime = new TimeSpan(0);
        }

        private void DrawBitmaps()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Bitmap image = new Bitmap(RenderEngine.Scene.Camera.Width, RenderEngine.Scene.Camera.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap overlight = new Bitmap(RenderEngine.Scene.Camera.Width, RenderEngine.Scene.Camera.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var imageData = new RawBitmap(image))
            using (var overlightData = new RawBitmap(overlight))
            {
                for (int y = 0; y < Height; y++) for (int x = 0; x < Width; x++)
                {
                    imageData.SetColor(x, y, Pixels[x][y]);
                    overlightData.SetColor(x, y, Pixels[x][y] - Color.White);
                    //overlightData.SetColor(x, y, new Color((float)Math.Log((Pixels[x][y] - Color.White).R), (float)Math.Log((Pixels[x][y] - Color.White).G), (float)Math.Log((Pixels[x][y] - Color.White).B)));
                }
            }

            foreach (var postEffect in RenderEngine.Configuration.PostEffects)
            {
                postEffect(image, overlight);
            }

            PostProcessingTime += watch.Elapsed;
            watch.Stop();
            image.SetResolution(RenderEngine.Configuration.Dpi, RenderEngine.Configuration.Dpi);
            _image = image;
            _overlight = overlight;
        }
    }
}
