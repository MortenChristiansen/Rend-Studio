using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;

namespace RayTracer.Plugins
{
    public class BilinearTextureFilter : ITextureFilter
    {
        protected Color[,] _colors;
        protected int _textureWidth;
        protected int _textureHeight;

        public BilinearTextureFilter()
        {

        }

        private BilinearTextureFilter(Color[,] pixels)
        {
            _colors = pixels;
            _textureWidth = pixels.GetLength(0);
            _textureHeight = pixels.GetLength(1);
        }

        #region ITextureFilter Members

        public Color GetTexelColor(float x, float y)
        {
            if (!Texture.CoordinateWithinBounds(ref x, ref y)) return Color.Red; //throw new IndexOutOfRangeException();
            //Fetch a bilinearly filtered texel
            float fu = (x + 1000.0f) * _textureWidth;
            float fv = (y + 1000.0f) * _textureHeight;
            int u1 = ((int)fu) % _textureWidth;
            int v1 = ((int)fv) % _textureHeight;
            int u2 = (u1 + 1) % _textureWidth;
            int v2 = (v1 + 1) % _textureHeight;
            //Calculate the fractional parts of u and v
            float fracu = fu - (float)Math.Floor(fu);
            float fracv = fv - (float)Math.Floor(fv);
            //Calculate weight factors
            float w1 = (1 - fracu) * (1 - fracv);
            float w2 = fracu * (1 - fracv);
            float w3 = (1 - fracu) * fracv;
            float w4 = fracu * fracv;
            //Fetch four texels
            Color c1 = _colors[u1, _textureHeight - 1 - v1];
            Color c2 = _colors[u2, _textureHeight - 1 - v1];
            Color c3 = _colors[u1, _textureHeight - 1 - v2];
            Color c4 = _colors[u2, _textureHeight - 1 - v2];
            //Scale and sum the four colors
            c1 = Color.Multiply(c1, w1);
            c2 = Color.Multiply(c2, w2);
            c3 = Color.Multiply(c3, w3);
            c4 = Color.Multiply(c4, w4);
            return Color.Add(c1, Color.Add(c2, Color.Add(c3, c4)));
        }

        public ITextureFilter GetFilterInstance(Color[,] pixels)
        {
            return new BilinearTextureFilter(pixels);
        }

        #endregion

        #region IPlugin Members

        public string Name
        {
            get { return "Bilinear Filtering"; }
        }

        #endregion
    }
}
