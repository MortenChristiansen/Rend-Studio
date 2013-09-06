using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;

namespace RayTracer.Geometry
{
    public class LinearInterpolation : IPath
    {
        private List<Vector> _points;

        public LinearInterpolation(Vector startPoint)
        {
            _points = new List<Vector>(2);
            _points.Add(startPoint);
        }

        public LinearInterpolation(List<Vector> points)
        {
            _points = points;
        }

        public void AddPoint(Vector point)
        {
            _points.Add(point);
        }

        /// <summary>
        /// Interpolates the point along the path at the given relative distance
        /// (from 0 to 1).
        /// </summary>
        /// <param name="distance">The relative distance along the entire path</param>
        /// <returns>The point in 3D space along the path</returns>
        public Vector GetValue(float distance)
        {
            if (_points.Count == 1) return _points[0];

            distance -= (int)distance;
            int segmentStart = (int)(distance * (_points.Count - 1));
            float relativeSegmentLength = 1f / (_points.Count - 1);
            float relativeSegmentPoint = (distance - relativeSegmentLength * segmentStart) / relativeSegmentLength;
            Vector a = _points[segmentStart];
            Vector b = _points[segmentStart + 1];

            return a + (b - a) * relativeSegmentPoint;
        }
    }
}