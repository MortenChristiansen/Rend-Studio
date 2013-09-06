using System;
using System.Drawing;
using System.IO;
using RayTracer.Engine;
using RayTracer.Plugins;
using RayTracer.PostProcessing;


namespace RayTracer.Geometry.Coloring
{
    public class Texture
    {
        private Color[,] _colors;
        private ITextureFilter _textureFilter;

        public static string TextureDirectory { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture(string textureFileName)
        {
            string dir = TextureDirectory == null ? string.Empty : TextureDirectory;
            Bitmap textureImage = new Bitmap(System.IO.Path.Combine(dir, textureFileName));
            Height = textureImage.Height;
            Width = textureImage.Width;
            _colors = new Color[Width, Height];

            using (RawBitmap imageData = new RawBitmap(textureImage))
            {
                for (int y = 0; y < Height; y++) for (int x = 0; x < Width; x++)
                {
                    _colors[x, y] = imageData.GetColor(x, y);
                }
            }
            if (RenderEngine.Configuration.TextureFilter != null)
            {
                _textureFilter = RenderEngine.Configuration.TextureFilter.GetFilterInstance(_colors);
            }
        }

        public Color GetTexelColor(float x, float y)
        {
            if (_textureFilter == null)
            {
                //Default texel color calculation
                if (!CoordinateWithinBounds(ref x, ref y)) throw new IndexOutOfRangeException();
                int actualY = Height - (int)(y * (Height - 1)) - 1;
                return _colors[(int)(x * Width - 1), actualY];
            }
            return _textureFilter.GetTexelColor(x, y);
        }

        internal static bool CoordinateWithinBounds(ref float x, ref float y)
        {
            float eps = 0.00001f;

            if (x > 1.0f + eps || x < 0.0f - eps || y > 1.0f + eps || y < 0.0f - eps)
            {
                return false;
            }
            x = x > 1 ? 1 : x;
            x = x < 0 ? 0 : x;
            y = y > 1 ? 1 : y;
            y = y < 0 ? 0 : y;

            return true;
        }
    }
}
