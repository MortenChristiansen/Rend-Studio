using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Geometry.Coloring
{
    public struct Color
    {
        private readonly float r;
        private readonly float g;
        private readonly float b;

        public byte R { get { return (byte)Math.Min(r, 255f); } }
        public byte G { get { return (byte)Math.Min(g, 255f); } }
        public byte B { get { return (byte)Math.Min(b, 255f); } }

        public ushort R16 { get { return (ushort)Math.Min(r * 256, ushort.MaxValue); } }
        public ushort G16 { get { return (ushort)Math.Min(g * 256, ushort.MaxValue); } }
        public ushort B16 { get { return (ushort)Math.Min(b * 256, ushort.MaxValue); } }

        public Color Inverted { get { return new Color(255 - R, 255 - G, 255 - B); } }
        public Color Overlight { get { return new Color(r - 255, g - 255, b - 255); } }
        public Color GreyScale { get { return new Color(r * .299f + g * .587f + b * .114f); } }
        public Color Clamped { get { return new Color(R, G, B); } }

        public Color(float r, float g, float b)
        {
            this.r = r >= 0 ? r : 0;
            this.g = g >= 0 ? g : 0;
            this.b = b >= 0 ? b : 0;
        }

        public Color(float grey)
        {
            float greyVal = grey < 0 ? 0 : grey;
            this.r = greyVal;
            this.g = greyVal;
            this.b = greyVal;
        }

        public float GetAverageValue()
        {
            return (r + g + b) / 3f;
        }

        public static Color HighRangeColor(ushort r, ushort g, ushort b)
        {
            return new Color(((float)r) / 256, ((float)g) / 256, ((float)b) / 256);
        }

        public static implicit operator Color(System.Windows.Media.Color color)
        {
            return new Color(color.R, color.G, color.B);
        }

        public static implicit operator Color(System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B);
        }

        /// <summary>
        /// Adds each component of two colors together.
        /// </summary>
        /// <param name="a">The first color</param>
        /// <param name="b">The second color</param>
        /// <returns>The addition of the two colors</returns>
        public static Color Add(Color a, Color b)
        {
            return new Color(a.r + b.r, a.g + b.g, a.b + b.b);
        }

        /// <summary>
        /// Subtracts each component of two colors together.
        /// </summary>
        /// <param name="a">The first color</param>
        /// <param name="b">The color to subtract</param>
        /// <returns>The subtraction of the two colors</returns>
        public static Color Subtract(Color a, Color b)
        {
            return new Color(a.r - b.r, a.g - b.g, a.b - b.b);
        }

        /// <summary>
        /// Mutliplies each component of a color by a specified coefficient.
        /// </summary>
        /// <param name="a">The original color</param>
        /// <param name="coefficient">The multiplication coefficient</param>
        /// <returns>The multiplied color</returns>
        public static Color Multiply(Color a, float coefficient)
        {
            coefficient = Math.Abs(coefficient);
            return new Color(a.r * coefficient, a.g * coefficient, a.b * coefficient);
        }

        /// <summary>
        /// Filters each component of the color by the comonent of the filter color. A black
        /// filter would filter all colors and a white filter would not filter any color.
        /// </summary>
        /// <param name="filter">The filter color</param>
        /// <returns>The filtered color</returns>
        public Color Filter(Color filter)
        {
            return new Color(filter.r * (r / 255f), filter.g * (g / 255f), filter.b * (b / 255f));
        }

        #region Operators

        public static Color operator +(Color a, Color b)
        {
            return new Color(a.r + b.r, a.g + b.g, a.b + b.b);
        }

        public static Color operator -(Color a, Color b)
        {
            return new Color(a.r - b.r, a.g - b.g, a.b - b.b);
        }

        public static Color operator *(Color a, float coefficient)
        {
            coefficient = Math.Abs(coefficient);
            return new Color(a.r * coefficient, a.g * coefficient, a.b * coefficient);
        }

        #endregion

        #region Static Colors

        public static readonly Color Black = new Color(0f);
        public static readonly Color White = new Color(255f);
        public static readonly Color Red = new Color(255f, 0f, 0f);
        public static readonly Color Green = new Color(0f, 255f, 0f);
        public static readonly Color Blue = new Color(0f, 0f, 255f);
        public static readonly Color Yellow = new Color(255f, 255f, 0f);
        public static readonly Color Magenta = new Color(255f, 0f, 255f);
        public static readonly Color Cyan = new Color(0f, 255f, 255f);

        #endregion
    }
}
