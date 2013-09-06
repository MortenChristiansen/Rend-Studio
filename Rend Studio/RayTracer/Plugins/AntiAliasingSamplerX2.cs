using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Engine;
using RayTracer.Engine.Screen;
using RayTracer.Geometry;

namespace RayTracer.Plugins
{
    public class AntiAliasingSamplerX2 : ISuperSampler
    {
        protected int _samples;

        public AntiAliasingSamplerX2()
        {
            _samples = 2;
        }

        #region ISuperSampler Members

        public Color[] RenderRays(Ray[] rays)
        {
            Color[] colors = new Color[rays.Length];
            Camera camera = RenderEngine.Scene.Camera;
            float cameraFocusLength = (float)Math.Sqrt(camera.Width * camera.Width + camera.Height * camera.Height);

            int sampleMin = -_samples / 2;
            int samepleMax = (_samples / 2) + _samples % 2;
            for (int rayId = 0; rayId < rays.Length; rayId++)
            {
                colors[rayId] = Color.Black;

                float increment = 1f / _samples;
                for (int x = sampleMin; x < samepleMax; x++) for (int y = sampleMin; y < samepleMax; y++)
                {
                    Color color = Color.Black;
                    float distance;
                    Ray ray = new Ray(rays[rayId].Origin, (rays[rayId].Direction * camera.Zoom * cameraFocusLength + camera.XAxis * increment * x + camera.YAxis * increment * y).Normal);
                    RenderEngine.RenderUnit.RayTrace(ray, ref color, 0, 1, out distance);
                    colors[rayId] += color.Clamped;
                }

                colors[rayId] *= 1f / (_samples * _samples);
            }

            return colors;
        }

        #endregion

        #region IPlugin Members

        public string Name
        {
            get { return "Anti Aliasing x2"; }
        }

        #endregion
    }
}
