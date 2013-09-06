using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Mathematics
{
    public static class FloatExtensions
    {
        private const float MAX_ALLOWED_DIFFERENCE = 0.001f;

        public static bool AlmostEqual(this float a, float b)
        {
            return Math.Abs(a - b) <= MAX_ALLOWED_DIFFERENCE;
        }

        public static float Clamp(this float val, float min, float max)
        {
            return val < min ? min : val > max ? max : val;
        }
    }
}
