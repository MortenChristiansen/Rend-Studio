//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
////using System.Runtime.InteropServices;

//namespace RayTracer.Mathematics
//{
//    //[StructLayout(LayoutKind.Sequential, Size = VectorD.Size, Pack = 0), Serializable]
//    //[StructLayout(LayoutKind.Sequential)]
//    public struct Vector
//    {
//        /*
//         [StructLayout( LayoutKind.Explicit)
//        class MyUnio

//        [FieldOffset(0)] byte byte1
//        [FieldOffset(1)] byte byte2
//        [FieldOffset(2)] byte byte3
//        [FieldOffset(3)] byte byte4
//        [FieldOffset(0)] int myInt
//         */

//        public const double EPSILON = 0.000001f;
//        public static readonly Vector XUnit = new Vector(1.0d, 0.0d, 0.0d);
//        public static readonly Vector YUnit = new Vector(0.0d, 1.0d, 0.0d);
//        public static readonly Vector ZUnit = new Vector(0.0d, 0.0d, 1.0d);
//        public static readonly Vector Unit = new Vector(1.0d, 1.0d, 1.0d);
//        public static readonly Vector Zero = new Vector(0.0d, 0.0d, 0.0d);

//        //[MarshalAs(UnmanagedType.R4)] //Used on private fields
//        public double X;
//        public double Y;
//        public double Z;
//        //public double LengthProp { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }

//        public static readonly byte[] NEXT_AXIS = new byte[] { 1, 2, 0 };
//        public static readonly byte[] PREV_AXIS = new byte[] { 2, 0, 1 };

//        public Vector(double x, double y, double z) : this()
//        {
//            X = x;
//            Y = y;
//            Z = z;
//        }

//        public void Minimize(Vector v)
//        {
//            X = Math.Min(v.X, X);
//            Y = Math.Min(v.Y, Y);
//            Z = Math.Min(v.Z, Z);
//        }

//        public void Maximize(Vector v)
//        {
//            X = Math.Max(v.X, X);
//            Y = Math.Max(v.Y, Y);
//            Z = Math.Max(v.Z, Z);
//        }

//        public void Normalize()
//        {
//            this *= 1.0d / Math.Sqrt(X * X + Y * Y + Z * Z);
//        }

//        public double Length()
//        {
//            return Math.Sqrt(X * X + Y * Y + Z * Z);
//        }

//        public double LengthSq()
//        {
//            return X * X + Y * Y + Z * Z;
//        }
        
//        public Vector Rotate(byte axis, double radians)
//        {
//            //Rotates clockwise, when looking in the positive direction of the rotation axis
//            byte next = NEXT_AXIS[axis];
//            byte prev = PREV_AXIS[axis];
//            //Optimizing common cases
//            if (radians == 0.0d || radians == Math.PI * 2.0d || (this[next] == 0.0d && this[prev] == 0.0d)) return this;
//            if (radians == Math.PI * 0.5d) return this.InverAxis(prev);
//            else if (radians == Math.PI) return this.InverAxis(next).InverAxis(prev);
//            else if (radians == Math.PI * 1.5d) return this.InverAxis(next);

//            Vector v = Vector.SetValue(this, axis, 0);
//            double tmpLength = v.Length();
//            v.Normalize();
//            double currentAngle = Math.Acos(v[prev]);
//            if (v[next] < 0.0d) currentAngle = Math.PI * 2.0d - currentAngle;
//            double newPrev = Math.Cos(radians + currentAngle);
//            double newNext = Math.Sin(radians + currentAngle);
//            v[next] = newNext;
//            v[prev] = newPrev;
//            v *= tmpLength;
//            v[axis] = this[axis];
//            return v;
//        }

//        public Vector InverAxis(byte axis)
//        {
//            return Vector.SetValue(this, axis, -this[axis]);
//        }

//        public static Vector Minimize(Vector a, Vector b)
//        {
//            a.X = Math.Min(a.X, b.X);
//            a.Y = Math.Min(a.Y, b.Y);
//            a.Z = Math.Min(a.Z, b.Z);
//            return a;
//        }

//        public static Vector Maximize(Vector a, Vector b)
//        {
//            a.X = Math.Max(a.X, b.X);
//            a.Y = Math.Max(a.Y, b.Y);
//            a.Z = Math.Max(a.Z, b.Z);
//            return a;
//        }

//        public static Vector Normalize(Vector v)
//        {
//            v *= 1.0d / v.Length();
//            return v;
//        }

//        public static Vector Normal(Vector a, Vector b)
//        {
//            double dX = a.Y * b.Z - a.Z * b.Y;
//            double dY = a.Z * b.X - a.X * b.Z;
//            double dZ = a.X * b.Y - a.Y * b.X;
//            double factor = 1.0d / Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
//            a.X = dX * factor;
//            a.Y = dY * factor;
//            a.Z = dZ * factor;
//            return a;
//        }

//        public static bool Close(Vector a, Vector b)
//        {
//            return Math.Abs(a.X - b.X) < EPSILON && Math.Abs(a.Y - b.Y) < EPSILON && Math.Abs(a.Z - b.Z) < EPSILON;
//        }

//        public static bool Parallel(Vector a, Vector b)
//        {
//            double factor = a.X * b.X;
//            return b.Y == factor * a.Y && b.Z == factor * a.Z;
//        }

//        public static double Angle(Vector a, Vector b)
//        {
//            a.Normalize();
//            b.Normalize();
//            return Math.Acos(a * b);
//        }

//        public static double Dot(Vector a, Vector b)
//        {
//            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
//        }

//        public static Vector Cross(Vector u, Vector v)
//        {
//            Vector res;
//            res.X = u.Y * v.Z - u.Z * v.Y;
//            res.Y = u.Z * v.X - u.X * v.Z;
//            res.Z = u.X * v.Y - u.Y * v.X;
//            return res;
//        }

//        public void Invert()
//        {
//            X *= -1.0d;
//            Y *= -1.0d;
//            Z *= -1.0d;
//        }
    
//        public Vector Reflect(Vector normal)
//        {
//            return this - (2.0d * Vector.Dot(this, normal) * normal);
//        }

//        public double this[byte axis]
//        {
//            get
//            {
//                unsafe
//                {
//                    fixed (double* pX = &X)
//                        return *(pX + axis);
//                }
//            }
//            set
//            {
//                unsafe
//                {
//                    fixed (double* pX = &X)
//                        *(pX + axis) = value;
//                }
//            }
//        }

//        public static Vector SetValue(Vector v, byte axis, double value)
//        {
//            v[axis] = value;
//            return v;
//        }

//        public static Vector AddValue(Vector v, byte axis, double value)
//        {
//            v[axis] += value;
//            return v;
//        }

//        public static Vector[] GetConeScatterVectors(Vector v, double radius, int amountFactor)
//        {
//            Vector[] shadowRays = new Vector[amountFactor * 4];
//            Vector normal1 = Vector.Cross(v, Vector.SetValue(v, 0, -v.X));
//            Vector normal2 = Vector.Cross(v, normal1);
//            normal1.Normalize();
//            normal2.Normalize();
//            MersenneTwister rnd = new MersenneTwister(true);
//            double sectionSize = radius / amountFactor;

//            for (int i = 0; i < amountFactor; i++)
//            {
//                double min = i * sectionSize;
//                for (int j = 0; j < 4; j++)
//                {
//                    Vector offset = new Vector();
//                    double lengthFactor = (rnd.Rand() / (double)amountFactor) + ((double)i / (double)amountFactor);
//                    double length1 = rnd.Rand() * radius;
//                    double length2 = Math.Sqrt((radius * radius) - (length1 * length1));
//                    if (j == 0)
//                    {
//                        offset = (normal1 * length1 + normal2 * length2) * lengthFactor;
//                    }
//                    else if (j == 1)
//                    {
//                        offset = (normal1 * length1 - normal2 * length2) * lengthFactor;
//                    }
//                    else if (j == 2)
//                    {
//                        offset = (-normal1 * length1 + normal2 * length2) * lengthFactor;
//                    }
//                    else
//                    {
//                        offset = (-normal1 * length1 - normal2 * length2) * lengthFactor;
//                    }
//                    shadowRays[i * 4 + j] = Vector.Normalize(v + offset);
//                }
//            }

//            return shadowRays;
//        }

//        public static double operator *(Vector a, Vector b)
//        {
//            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
//        }

//        public static Vector operator *(Vector a, double b)
//        {
//            a.X *= b;
//            a.Y *= b;
//            a.Z *= b;
//            return a;
//        }

//        public static Vector operator *(double a, Vector b)
//        {
//            b.X *= a;
//            b.Y *= a;
//            b.Z *= a;
//            return b;
//        }

//        public static Vector operator +(Vector a, Vector b)
//        {
//            a.X += b.X;
//            a.Y += b.Y;
//            a.Z += b.Z;
//            return a;
//        }

//        public static Vector operator -(Vector a, Vector b)
//        {
//            a.X -= b.X;
//            a.Y -= b.Y;
//            a.Z -= b.Z;
//            return a;
//        }

//        public static Vector operator -(Vector v)
//        {
//            v.X *= -1.0d;
//            v.Y *= -1.0d;
//            v.Z *= -1.0d;
//            return v;
//        }

//        public static bool operator ==(Vector a, Vector b)
//        {
//            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
//        }

//        public static bool operator !=(Vector a, Vector b)
//        {
//            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
//        }

//        public override bool Equals(object obj)
//        {
//            return obj is Vector && this == ((Vector)obj);
//        }

//        public override int GetHashCode()
//        {
//            return (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode().GetHashCode());
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Mathematics
{
    public struct Vector
    {
        public const float Epsilon = 0.0048f; //Any less messes with shadows
        public static readonly Vector XUnit = new Vector(1, 0, 0);
        public static readonly Vector YUnit = new Vector(0, 1, 0);
        public static readonly Vector ZUnit = new Vector(0, 0, 1);
        public static readonly Vector Unit = new Vector(1, 1, 1);
        public static readonly Vector Zero = new Vector(0, 0, 0);

        public static readonly Vector Min = new Vector(float.MinValue);
        public static readonly Vector Max = new Vector(float.MaxValue);

        private float _x;
        private float _y;
        private float _z;

        public float X { get { return _x; } private set { _x = value; } }
        public float Y { get { return _y; } private set { _y = value; } }
        public float Z { get { return _z; } private set { _z = value; } }

        public static readonly byte[] NextAxis = new byte[] { 1, 2, 0 };
        public static readonly byte[] PreviousAxis = new byte[] { 2, 0, 1 };

        /// <summary>
        /// Gets the normal the vector.
        /// </summary>
        public Vector Normal
        {
            get { return this * (1.0f / (float)Math.Sqrt(X * X + Y * Y + Z * Z)); }
        }

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public float Length
        {
            get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        /// <summary>
        /// Gets the length of the vector squered.
        /// </summary>
        public float LengthSquared
        {
            get { return X * X + Y * Y + Z * Z; }
        }

        public Vector(float x, float y, float z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector(float dimension)
            : this()
        {
            X = dimension;
            Y = dimension;
            Z = dimension;
        }

        /// <summary>
        /// Rotates the vector around a given axis and returns the result.
        /// </summary>
        /// <param name="axis">The X-, Y- or Z-axis, represented by 0-2</param>
        /// <param name="radians">The number of radians to rotate</param>
        /// <returns>The rotated vector</returns>
        public Vector Rotate(byte axis, float radians)
        {
            //Rotates clockwise, when looking in the positive direction of the rotation axis
            byte next = NextAxis[axis];
            byte prev = PreviousAxis[axis];
            //Optimizing common cases
            if (radians == 0 || radians == Math.PI * 2 || (this[next] == 0 && this[prev] == 0)) return this;
            if (radians == Math.PI * 0.5f) return this.InverAxis(prev);
            else if (radians == Math.PI) return this.InverAxis(next).InverAxis(prev);
            else if (radians == Math.PI * 1.5f) return this.InverAxis(next);

            Vector v = Vector.SetValue(this, axis, 0);
            float tmpLength = v.Length;
            v = v.Normal;
            float currentAngle = (float)Math.Acos(v[prev]);
            if (v[next] < 0) currentAngle = (float)Math.PI * 2 - currentAngle;
            float newPrev = (float)Math.Cos(radians + currentAngle);
            float newNext = (float)Math.Sin(radians + currentAngle);
            v = Vector.SetValue(v, next, newNext);
            v = Vector.SetValue(v, prev, newPrev);
            v *= tmpLength;
            v = Vector.SetValue(v, axis, axis);
            return v;
        }

        /// <summary>
        /// Rotates the vector around a given axis perpendicular to the vector and returns the result.
        /// </summary>
        /// <param name="axis">The perpendicular vector to rotate about</param>
        /// <param name="radians">The number of radians to rotate</param>
        /// <returns>The rotated vector</returns>
        public Vector Rotate(Vector axis, float radians)
        {
            float theta = radians; // Math.PI;

            float c = (float)Math.Cos(theta);
            float s = (float)Math.Sin(theta);
            float t = 1 - (float)Math.Cos(theta);

            Matrix3 mxRotate = new Matrix3(
                    t * (float)Math.Pow(axis.X, 2.0) + c, t * axis.X * axis.Y - s * axis.Z, t * axis.X * axis.Z + s * axis.Y,
                    t * axis.X * axis.Y + s * axis.Z, t * (float)Math.Pow(axis.Y, 2.0) + c, t * axis.Y * axis.Z - s * axis.X,
                    t * axis.X * axis.Z - s * axis.Y, t * axis.Y * axis.Z + s * axis.X, t * (float)Math.Pow(axis.Z, 2.0) + c);

            return mxRotate * this;
        }

        /// <summary>
        /// Gets the vector with one axis inverted.
        /// </summary>
        /// <param name="axis">The X-, Y- or Z-axis, represented by 0-2</param>
        /// <returns>The vector with inverted axis</returns>
        public Vector InverAxis(byte axis)
        {
            return Vector.SetValue(this, axis, -this[axis]);
        }

        /// <summary>
        /// Scales each dimension of the vector by the dimension of a scaling vector.
        /// </summary>
        /// <param name="scale">Scaling vector</param>
        /// <returns>The scaled vector</returns>
        public Vector Scale(Vector scale)
        {
            return new Vector(X * scale.X, Y * scale.Y, Z * scale.Z);
        }

        /// <summary>
        /// Gets the inverted vector.
        /// </summary>
        /// <returns>The inverted vector</returns>
        public Vector Invert()
        {
            return new Vector(-X, -Y, -Z);
        }

        /// <summary>
        /// Mirros the vector across a normal vector.
        /// </summary>
        /// <param name="normal">The mirroring vector</param>
        /// <returns>The reflected vector</returns>
        public Vector Reflect(Vector normal)
        {
            return this - (2.0f * Vector.Dot(this, normal) * normal);
        }

        /// <summary>
        /// Gets the value of a single dimension of the vector.
        /// </summary>
        /// <param name="axis">The X-, Y- or Z-axis, represented by 0-2</param>
        /// <returns>The single-dimension value</returns>
        public float this[byte axis]
        {
            get
            {
                unsafe
                {
                    fixed (float* pX = &_x)
                        return *(pX + axis);
                }
            }
        }

        /// <summary>
        /// Determines if the vector has any invalid values.
        /// </summary>
        /// <returns>Whether one or more components of the vector are NaN</returns>
        public bool IsNan()
        {
            return float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z);
        }

        #region Static Methods

        public static Vector Lerp(Vector value1, Vector value2, float amount)
        {
            return new Vector(
                value1.X + ((value2.X - value1.X) * amount),
                value1.Y + ((value2.Y - value1.Y) * amount),
                value1.Z + ((value2.Z - value1.Z) * amount)
            );
        }

        /// <summary>
        /// Gets the projection of a unto b.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The projected vector</returns>
        public static Vector Project(Vector a, Vector b)
        {
            return (Vector.Dot(a, b) / b.LengthSquared) * b;
        }

        /// <summary>
        /// Creates a vector with the smallest value of each axis, from two source vectors.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The minimized vector</returns>
        public static Vector Minimize(Vector u, Vector v)
        {
            return new Vector(Math.Min(v.X, u.X), Math.Min(v.Y, u.Y), Math.Min(v.Z, u.Z));
        }

        /// <summary>
        /// Creates a vector with the biggest value of each axis, from two source vectors.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The maximized vector</returns>
        public static Vector Maximize(Vector u, Vector v)
        {
            return new Vector(Math.Max(v.X, u.X), Math.Max(v.Y, u.Y), Math.Max(v.Z, u.Z));
        }

        /// <summary>
        /// Creates a vector with a value replaced for a given axis.
        /// </summary>
        /// <param name="v">The original vector</param>
        /// <param name="axis">The axis of which the value is replaced</param>
        /// <param name="value">The new value</param>
        /// <returns>The modified vector</returns>
        public static Vector SetValue(Vector v, byte axis, float value)
        {
            var values = new float[] { v.X, v.Y, v.Z };
            values[axis] = value;
            return new Vector(values[0], values[1], values[2]);
        }

        /// <summary>
        /// Creates a vector with a value added for a given axis.
        /// </summary>
        /// <param name="v">The original vector</param>
        /// <param name="axis">The axis of which the value is added</param>
        /// <param name="value">The value to add</param>
        /// <returns>The modified vector</returns>
        public static Vector AddValue(Vector v, byte axis, float value)
        {
            var values = new float[] { v.X, v.Y, v.Z };
            values[axis] += value;
            return new Vector(values[0], values[1], values[2]);
        }

        /// <summary>
        /// Gets the normal vector for two vectors.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The normal vector</returns>
        public static Vector NormalVector(Vector a, Vector b)
        {
            return new Vector
                (
                    a.Y * b.Z - a.Z * b.Y,
                    a.Z * b.X - a.X * b.Z,
                    a.X * b.Y - a.Y * b.X
                ).Normal;
        }

        /// <summary>
        /// Determines if two vectors are almost identical.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>Whether the two vectors are almost identical</returns>
        public static bool Close(Vector a, Vector b)
        {
            return Math.Abs(a.X - b.X) < Epsilon && Math.Abs(a.Y - b.Y) < Epsilon && Math.Abs(a.Z - b.Z) < Epsilon;
        }

        /// <summary>
        /// Determines if two vectors travel in the same direction.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>Whether the two vectors are parallel</returns>
        public static bool Parallel(Vector a, Vector b)
        {
            float factor = a.X * b.X;
            return b.Y == factor * a.Y && b.Z == factor * a.Z;
        }

        /// <summary>
        /// Gets the angle betwenn two vectors.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The angle between the two vectors</returns>
        public static float Angle(Vector a, Vector b)
        {
            return (float)Math.Acos(a.Normal * b.Normal);
        }

        /// <summary>
        /// Calculates the dot product between two vectors.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The dot product of the two vectors</returns>
        public static float Dot(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Calculates the cross product between two vectors.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The cross product of the two vectors</returns>
        public static Vector Cross(Vector a, Vector b)
        {
            return new Vector
            (
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        /// <summary>
        /// Gets an array of vectors distributed randomly in a cone shape. A semi-even
        /// distribution is guaranteed in a number of layers.
        /// </summary>
        /// <param name="v">The central direction of the cone, in the widening direction. The length
        /// of the vector is the length ofthe cone.</param>
        /// <param name="radius">The radius of the cone in at the widest point.</param>
        /// <param name="amountFactor">Determines the number of layers of vectors which
        /// are created. There are four vectors in each layer.</param>
        /// <returns>The vectors making up the cone.</returns>
        public static Vector[] GetConeScatterVectors(Vector v, float radius, int amountFactor)
        {
            Vector[] shadowRays = new Vector[amountFactor * 4];
            Vector normal1 = Vector.Cross(v, Vector.SetValue(v, 0, -v.X)).Normal;
            Vector normal2 = Vector.Cross(v, normal1).Normal;
            MersenneTwister rnd = new MersenneTwister(true);
            float sectionSize = radius / amountFactor;

            for (int i = 0; i < amountFactor; i++)
            {
                float min = i * sectionSize;
                for (int j = 0; j < 4; j++)
                {
                    Vector offset = new Vector();
                    float lengthFactor = (rnd.Rand() / (float)amountFactor) + ((float)i / (float)amountFactor);
                    float length1 = rnd.Rand() * radius;
                    float length2 = (float)Math.Sqrt((radius * radius) - (length1 * length1));
                    if (j == 0)
                    {
                        offset = (normal1 * length1 + normal2 * length2) * lengthFactor;
                    }
                    else if (j == 1)
                    {
                        offset = (normal1 * length1 - normal2 * length2) * lengthFactor;
                    }
                    else if (j == 2)
                    {
                        offset = (-normal1 * length1 + normal2 * length2) * lengthFactor;
                    }
                    else
                    {
                        offset = (-normal1 * length1 - normal2 * length2) * lengthFactor;
                    }
                    shadowRays[i * 4 + j] = (v + offset).Normal;
                }
            }

            return shadowRays;
        }

        #endregion

        #region Operators

        public static float operator *(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector operator *(Vector a, float b)
        {
            return new Vector(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector operator *(float a, Vector b)
        {
            return new Vector(b.X * a, b.Y * a, b.Z * a);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector operator -(Vector v)
        {
            return new Vector(-v.X, -v.Y, -v.Z);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector && this == ((Vector)obj);
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode().GetHashCode());
        }

        #endregion
    }
}
