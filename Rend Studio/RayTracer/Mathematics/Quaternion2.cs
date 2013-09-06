//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace RayTracer.Mathematics
//{
//    public class Quaternion2
//    {
//        #region Member Variables

//        private const double PIOVER180 = Math.PI / 180;

//        private double[] values;

//        #endregion

//        #region Constructors

//        /**
//        * Default Constructor
//        */
//        public Quaternion2()
//        {
//            values = new double[4];
//            values[0] = 0;
//            values[1] = 0;
//            values[2] = 0;
//            values[3] = 1;
//        }

//        /**
//         * Copy Constructor
//         */
//        public Quaternion2(Quaternion2 quaternion)
//        {
//            values = new double[4];

//            values[0] = quaternion[0];
//            values[1] = quaternion[1];
//            values[2] = quaternion[2];
//            values[3] = quaternion[3];
//        }

//        /**
//         * Parameterized Constructor
//         */
//        public Quaternion2(double x, double y, double z, double w)
//        {
//            values = new double[4];
//            values[0] = x;
//            values[1] = y;
//            values[2] = z;
//            values[3] = w;
//        }

//        #endregion

//        #region Indexer

//        /**
//        * Indexer
//        */
//        public double this[int index]
//        {
//            get
//            {
//                return this.values[index];
//            }
//            set
//            {
//                this.values[index] = value;
//            }
//        }

//        #endregion

//        #region Operator Overloaders

//        /**
//        * Multiplication with other quaternion
//        */
//        public static Quaternion2 operator *(Quaternion2 left, Quaternion2 right)
//        {
//            Quaternion2 ret = new Quaternion2();

//            ret[0] = left[1] * right[2] - left[2] * right[1] + left[3] * right[0] + left[0] * right[3];
//            ret[1] = left[2] * right[0] - left[0] * right[2] + left[3] * right[1] + left[1] * right[3];
//            ret[2] = left[0] * right[1] - left[1] * right[0] + left[3] * right[2] + left[2] * right[3];
//            ret[3] = left[3] * right[3] - left[0] * right[0] - left[1] * right[1] - left[2] * right[2];

//            return ret;
//        }

//        /**
//         * Multiplication with scalar
//         */
//        public static Quaternion2 operator *(Quaternion2 quat, double scalar)
//        {
//            Quaternion2 ret = new Quaternion2();
//            ret[0] = quat[0] * scalar;
//            ret[1] = quat[1] * scalar;
//            ret[2] = quat[2] * scalar;
//            ret[3] = quat[3] * scalar;
//            return ret;
//        }

//        /**
//         * Addition operator
//         */
//        public static Quaternion2 operator +(Quaternion2 left, Quaternion2 right)
//        {
//            Quaternion2 ret = new Quaternion2();

//            ret.values[0] = left[0] + right[0];
//            ret.values[1] = left[1] + right[1];
//            ret.values[2] = left[2] + right[2];
//            ret.values[3] = left[3] + right[3];

//            return ret;
//        }

//        #endregion

//        #region Methods

//        /**
//                 * Load Identity Quaternion
//                 */
//        public void loadIdentity()
//        {
//            values[0] = 0.0;
//            values[1] = 0.0;
//            values[2] = 0.0;
//            values[3] = 1.0;
//        }

//        /**
//         * Return an inverse quaternion
//         * @remark
//         *              The math is:  q* \ |q| where q* is the conjugate of the quaternion
//         *              and |q| is the magnitude, but since we only work with unit quaternions
//         *              we can skip the divide.
//         * @return The conjugate quaternion to this
//         */
//        public Quaternion2 getInverse()
//        {
//            return getConjugate();
//        }

//        /**
//         * Return the conjugate quaternion
//         * @remark
//         *              The math: q* = -x, -y, -z, w
//         */
//        public Quaternion2 getConjugate()
//        {
//            Quaternion2 ret = new Quaternion2();
//            ret[0] = -this[0];
//            ret[1] = -this[1];
//            ret[2] = -this[2];
//            ret[3] = this[3];

//            return ret;
//        }

//        public double dotProduct(Quaternion2 other)
//        {
//            return values[0] * other[0] + values[1] * other[1] + values[2] * other[2] + values[3] * other[3];
//        }

//        /**
//         * Normalize the quaternion
//         */
//        public void normalize()
//        {
//            double norm = values[0] * values[0] + values[1] * values[1] + values[2] * values[2] + values[3] * values[3];
//            values[0] = values[0] / norm;
//            values[1] = values[1] / norm;
//            values[2] = values[2] / norm;
//            values[3] = values[3] / norm;
//        }

//        /**
//         * Linear Interpolation
//         */
//        public static Quaternion2 lerp(Quaternion2 from, Quaternion2 to, double time)
//        {
//            Quaternion2 ret = from * (1 - time) + to * time;
//            ret.normalize();
//            return ret;
//        }

//        /**
//         * Spherical Interpolation with no inversion
//         */
//        public static Quaternion2 slerpNoInvert(Quaternion2 from, Quaternion2 to, double time)
//        {
//            double cosAngle = from.dotProduct(to);
//            if (cosAngle < 0.95 && cosAngle > -0.95)
//            {
//                double angle = Math.Acos(cosAngle);
//                double sinInvert = 1 / Math.Sin(angle);
//                double fromScale = Math.Sin(angle * (1 - time)) * sinInvert;
//                double toScale = Math.Sin(angle * time) * sinInvert;
//                return from * fromScale + to * toScale;
//            }
//            else
//            {
//                return lerp(from, to, time);
//            }
//        }

//        /**
//         * Setup the quaternion from an axis and an angle
//         * @param axis - Vector holding the axis
//         * @param angle - Double holding the angle of rotation
//         * @return The new quaternion
//         */
//        public Quaternion2 fromAxisAngle(Vector axis, double angle)
//        {
//            double halfAngle = angle / 2;
//            double sinAngle = Math.Sin(halfAngle * PIOVER180);
//            double cosAngle = Math.Cos(halfAngle * PIOVER180);

//            values[0] = sinAngle * axis[0];
//            values[1] = sinAngle * axis[1];
//            values[2] = sinAngle * axis[2];
//            values[3] = cosAngle;

//            return this;
//        }

//        /**
//         * Return a matrix equivalent to the quaternion.
//         * @remark
//         *      Assumes the quaternion is normalized (x^2 + y^2 + z^2 + w^2 = 1). No
//         *      test is performed to check.
//         */
//        public Matrix toMatrix()
//        {
//            Matrix ret = new Matrix();

//            // Row 1
//            ret[0] = 1 - 2 * (this.values[1] * this.values[1] + this.values[2] * this.values[2]);
//            ret[4] = 2 * (this.values[0] * this.values[1] - this.values[3] * this.values[2]);
//            ret[8] = 2 * (this.values[3] * this.values[1] + this.values[0] * this.values[2]);
//            ret[12] = 0;

//            // Row 2
//            ret[1] = 2 * (this.values[0] * this.values[1] + this.values[3] * this.values[2]);
//            ret[5] = 1 - 2 * (this.values[0] * this.values[0] + this.values[2] * this.values[2]);
//            ret[9] = 2 * (this.values[1] * this.values[2] - this.values[3] * this.values[0]);
//            ret[13] = 0;

//            // Row 3
//            ret[2] = 2 * (this.values[0] * this.values[2] - this.values[3] * this.values[1]);
//            ret[6] = 2 * (this.values[1] * this.values[2] + this.values[3] * this.values[0]);
//            ret[10] = 1 - 2 * (this.values[0] * this.values[0] + this.values[1] * this.values[1]);
//            ret[14] = 0;

//            // Row 4
//            ret[3] = 0;
//            ret[7] = 0;
//            ret[11] = 0;
//            ret[15] = 1;

//            return ret;
//        }

//        #endregion
//    }
//}
