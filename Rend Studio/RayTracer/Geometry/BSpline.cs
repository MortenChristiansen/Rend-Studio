using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;

namespace RayTracer.Geometry
{
    public class BSpline : IPath
    {
        #region Private fields

        private readonly List<Vector> _controlPoints;
        private readonly int _curveDegree;

        #endregion

        #region Constructors

        public BSpline(int curveDegree)
        {
            _controlPoints = new List<Vector>();
            _curveDegree = curveDegree;
        }

        #endregion

        #region Public methods

        public void AddPoint(Vector point)
        {
            _controlPoints.Add(point);
        }

        public void RemoveFirstControlPoint()
        {
            if (_controlPoints.Count > 0)
                _controlPoints.RemoveAt(0);
        }

        public Vector GetValue(float distance)
        {
            if (distance < 0 || distance > 1) throw new ArgumentException();
            return DeBoor(distance);
        }

        #endregion

        #region Private methods

        private Vector DeBoor(float u)
        {
            if (u < 0 || u > 1) throw new ArgumentException();
            int k, h, s, p = _curveDegree;
            k = h = s = 0;
            List<float> knots = BSplineBasis.GetBSplineKnots(_curveDegree, _controlPoints.Count);
            
            for (int i = 0; i < knots.Count; i++)
                if (NumericalHelper.IsFirstGreaterOrEqual(u, knots[i]) && u < knots[i + 1])
                {
                    k = i;
                    s = GetKnotMultiplicity(knots, i);
                    if (s > p) s = p;
                    if (NumericalHelper.AreEqual(u, knots[i]))
                    {
                        h = p - s;
                    }
                    else
                    {
                        h = p;
                        s = 0;
                    }
                    break;
                }

            int max = 2 * knots.Count;
            Vector[] oldPoints = new Vector[max], newPoints = new Vector[max];
            float t;
            if (k - p >= k - s) for (int i = k - s; i <= k - p; i++) newPoints[i] = _controlPoints[i];
            else for (int i = k - s; i >= k - p; i--) newPoints[i] = _controlPoints[i];
            newPoints.CopyTo(oldPoints, 0);

            for (int r = 1; r <= h; r++)
            {
                for (int i = k - p + r; i <= k - s; i++)
                {
                    t = (u - knots[i]) / (knots[i + p - r + 1] - knots[i]);
                    newPoints[i] = (1 - t) * oldPoints[i - 1];
                    newPoints[i] += t * oldPoints[i];
                }
                newPoints.CopyTo(oldPoints, 0);
            }
            return newPoints[k - s];
        }

        private int GetKnotMultiplicity(List<float> knots, int index)
        {
            int multiplicity = 1;
            for (int i = index + 1; i < knots.Count; i++)
                if (NumericalHelper.AreEqual(knots[i], knots[index])) multiplicity++;
                else break;
            for (int i = index - 1; i >= 0; i--)
                if (NumericalHelper.AreEqual(knots[i], knots[index])) multiplicity++;
                else break;
            return multiplicity;
        }

        #endregion

        #region Properties

        public int NumOfControlPoints
        {
            get { return _controlPoints == null ? 0 : _controlPoints.Count; }
        }

        public List<Vector> ControlPoints
        {
            get { return _controlPoints; }
        }

        #endregion
    }

    class BSplineBasis
    {
        #region Private static fields

        private static List<float> _knots;

        #endregion

        #region Public static methods

        public static List<float> GetBSplineKnots(int curveDegree, int numOfControlPoints)
        {
            if (curveDegree <= 0) throw new ArgumentException();
            if (numOfControlPoints <= 0) throw new ArgumentException();
            if (0 == curveDegree && 0 == numOfControlPoints) return _knots;
            _knots = new List<float>();
            int numOfKnots = (numOfControlPoints - 1) + curveDegree + 2;
            for (int i = 0; i < numOfKnots; i++)
            {
                if (i <= curveDegree)
                {
                    _knots.Add(0.0f);
                    continue;
                }
                if (i >= numOfKnots - curveDegree - 1)
                {
                    _knots.Add(1.0f);
                    continue;
                }
                float dist = 1.0f / (numOfKnots - 2 * curveDegree - 1);
                _knots.Add(dist * (i - curveDegree));
            }
            return _knots;
        }

        #endregion

        #region Private static methods

        private static float N_i_p(int i, int p, float u, int curveDegree, int numOfControlPoints)
        {
            if (i < 0 || p < 0)
                throw new ArgumentException();
            if (NumericalHelper.AreEqual(p, 0)) return N_i_0(i, u, curveDegree, numOfControlPoints);

            float u_i = GetIthKnot(i, curveDegree, numOfControlPoints),
                   u_i_plus_1 = GetIthKnot(i + 1, curveDegree, numOfControlPoints),
                   u_i_plus_p = GetIthKnot(i + p, curveDegree, numOfControlPoints),
                   u_i_plus_p_plus_1 = GetIthKnot(i + p + 1, curveDegree, numOfControlPoints);

            float first, second;
            first = second = 0;

            if (!NumericalHelper.AreEqual(u_i_plus_p, u_i))
                first = ((u - u_i) / (u_i_plus_p - u_i));

            if (!NumericalHelper.AreEqual(u_i_plus_p_plus_1, u_i_plus_1))
                second = ((u_i_plus_p_plus_1 - u) / (u_i_plus_p_plus_1 - u_i_plus_1));

            return first * N_i_p(i, p - 1, u, curveDegree, numOfControlPoints) +
                   second * N_i_p(i + 1, p - 1, u, curveDegree, numOfControlPoints);
        }

        private static float N_i_0(int i, float u, int curveDegree, int numOfControlPoints)
        {
            float u_i = GetIthKnot(i, curveDegree, numOfControlPoints),
                   u_i_plus_1 = GetIthKnot(i + 1, curveDegree, numOfControlPoints);
            return (NumericalHelper.IsFirstGreaterOrEqual(u, u_i) && u < u_i_plus_1) ? 1 : 0;
        }

        private static float GetIthKnot(int i, int curveDegree, int numOfControlPoints)
        {
            if (i < 0) throw new ArgumentException();
            if (curveDegree <= 0) throw new ArgumentException();
            if (numOfControlPoints <= 0) throw new ArgumentException();
            int numOfKnots = (numOfControlPoints - 1) + curveDegree + 2;
            if (i <= curveDegree) return 0.0f;
            if (i >= numOfKnots - curveDegree) return 1.0f;
            float dist = 1.0f / (numOfKnots - 2 * curveDegree - 1);
            return dist * (i - curveDegree);
        }

        #endregion
    }

    class NumericalHelper
    {
        private const float EpsilonFloatVerySmall = 0.0000000000001f;

        public static bool AreEqual(float first, float second)
        {
            return first > second - EpsilonFloatVerySmall &&
                   first < second + EpsilonFloatVerySmall;
        }

        public static bool IsFirstGreaterOrEqual(float first, float second)
        {
            return AreEqual(first, second) || first > second;
        }

        public static bool IsFirstSmalerOrEqual(float first, float second)
        {
            return AreEqual(first, second) || first < second;
        }
    }
}
