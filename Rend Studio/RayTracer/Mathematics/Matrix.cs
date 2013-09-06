using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Mathematics
{
    public class Matrix4
    {
        public float[][] data = new float[4][];

        public Matrix4()
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                data[i] = new float[4];
            }
        }

        public float this[int x, int y]
        {
            get
            {
                return data[x][y];
            }
        }

        public static Matrix4 ScalingMatrix(float scale)
        {
            return ScalingMatrix(new Vector(scale, scale, scale));
        }

        public static Matrix4 ScalingMatrix(Vector scale)
        {
            var matrix = new Matrix4();
            matrix.data[0][0] = scale.X;
            matrix.data[1][1] = scale.Y;
            matrix.data[2][2] = scale.Z;
            matrix.data[3][3] = 1;
            return matrix;
        }

        public static Matrix4 TranslationMatrix(Vector translation)
        {
            var matrix = new Matrix4();
            matrix.data[0][3] = translation.X;
            matrix.data[1][3] = translation.Y;
            matrix.data[2][3] = translation.Z;
            matrix.data[0][0] = 1;
            matrix.data[1][1] = 1;
            matrix.data[2][2] = 1;
            matrix.data[3][3] = 1;
            return matrix;
        }

        public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
        {
            var matrix = new Matrix4();

            for (int i = 0; i < 4; i++) for (int j = 0; j < 4; j++) 
            {
                for (int k = 0; k < 4; k++) for (int l = 0; l < 4; l++)
                {
                    matrix.data[i][j] += m1.data[k][i] * m2.data[l][j];
                }
            }
            return matrix;
        }

        public Vector Transform(Vector v)
        {
            return new Vector(
                    data[0][0] * v.X + data[0][1] * v.Y + data[0][2] * v.Z,
                    data[1][0] * v.X + data[1][1] * v.Y + data[1][2] * v.Z,
                    data[2][0] * v.X + data[2][1] * v.Y + data[2][2] * v.Z);
        }
    }

    public class Matrix3
    {
        public float[,] data = new float[3, 3];

        public Matrix3(float val00, float val01, float val02, float val10, float val11, float val12, float val20, float val21, float val22)
        {
            data[0, 0] = val00;
            data[0, 1] = val01;
            data[0, 2] = val02;
            data[1, 0] = val10;
            data[1, 1] = val11;
            data[1, 2] = val12;
            data[2, 0] = val20;
            data[2, 1] = val21;
            data[2, 2] = val22;
        }

        public static Vector operator *(Matrix3 mat, Vector vec)
        {
            return new Vector(
                    mat.data[0, 0] * vec.X + mat.data[0, 1] * vec.Y + mat.data[0, 2] * vec.Z,
                    mat.data[1, 0] * vec.X + mat.data[1, 1] * vec.Y + mat.data[1, 2] * vec.Z,
                    mat.data[2, 0] * vec.X + mat.data[2, 1] * vec.Y + mat.data[2, 2] * vec.Z);
        }

        //#region Member Variables

        //double[] values = new double[16];

        //#endregion

        //#region Constructors

        ///**
        //* Constructor. Loads the identity matrix
        //*/
        //public Matrix()
        //{
        //    // Empty
        //}

        //#endregion

        //#region Indexer

        ///**
        //* Indexer
        //*/
        //public double this[int index]
        //{
        //    get
        //    {
        //        return values[index];
        //    }
        //    set
        //    {
        //        values[index] = value;
        //    }
        //}

        //#endregion

        //#region Getters And Setters

        //public double[] getValues() { return values; }

        //#endregion

        //#region Operator Overloaders

        ///**
        //* Overloading the multiplication operator
        //*/
        //public static Matrix operator *(Matrix left, Matrix right)
        //{
        //    Matrix ret = new Matrix();

        //    // Row 1
        //    ret[0] = left[0] * right[0] + left[4] * right[1] + left[8] * right[2] + left[12] * right[3];
        //    ret[4] = left[0] * right[4] + left[4] * right[5] + left[8] * right[6] + left[12] * right[7];
        //    ret[8] = left[0] * right[8] + left[4] * right[9] + left[8] * right[10] + left[12] * right[11];
        //    ret[12] = left[0] * right[12] + left[4] * right[13] + left[8] * right[14] + left[12] * right[15];
        //    // Row 2
        //    ret[1] = left[1] * right[0] + left[5] * right[1] + left[9] * right[2] + left[13] * right[3];
        //    ret[5] = left[1] * right[4] + left[5] * right[5] + left[9] * right[6] + left[13] * right[7];
        //    ret[9] = left[1] * right[8] + left[5] * right[9] + left[9] * right[10] + left[13] * right[11];
        //    ret[13] = left[1] * right[12] + left[5] * right[13] + left[9] * right[14] + left[13] * right[15];
        //    // Row 3
        //    ret[2] = left[2] * right[0] + left[6] * right[1] + left[10] * right[2] + left[14] * right[3];
        //    ret[6] = left[2] * right[4] + left[6] * right[5] + left[10] * right[6] + left[14] * right[7];
        //    ret[10] = left[2] * right[8] + left[6] * right[9] + left[10] * right[10] + left[14] * right[11];
        //    ret[14] = left[2] * right[12] + left[6] * right[13] + left[10] * right[14] + left[14] * right[15];
        //    // Row 4
        //    ret[3] = left[3] * right[0] + left[7] * right[1] + left[11] * right[2] + left[15] * right[3];
        //    ret[7] = left[3] * right[4] + left[7] * right[5] + left[11] * right[6] + left[15] * right[7];
        //    ret[11] = left[3] * right[8] + left[7] * right[9] + left[11] * right[10] + left[15] * right[11];
        //    ret[15] = left[3] * right[12] + left[7] * right[13] + left[11] * right[14] + left[15] * right[15];

        //    return ret;
        //}

        ///**
        //* Multiplication with a vector
        //*/
        //public static Vector operator *(Matrix mat, Vector vec)
        //{
        //    return new Vector(
        //        mat[0] * vec[0] + mat[4] * vec[1] + mat[8] * vec[2] + mat[12] * 1,
        //        mat[1] * vec[0] + mat[5] * vec[1] + mat[9] * vec[2] + mat[13] * 1,
        //        mat[2] * vec[0] + mat[6] * vec[1] + mat[10] * vec[2] + mat[14] * 1);
        //}

        ///**
        //* Multiplication with a scalar
        //*/
        //public static Matrix operator *(Matrix mat, double scalar)
        //{
        //    Matrix ret = new Matrix();
        //    ret[0] = mat[0] * scalar; ret[4] = mat[4] * scalar; ret[8] = mat[8] * scalar; ret[12] = mat[12] * scalar;
        //    ret[1] = mat[1] * scalar; ret[5] = mat[5] * scalar; ret[9] = mat[9] * scalar; ret[13] = mat[13] * scalar;
        //    ret[2] = mat[2] * scalar; ret[6] = mat[6] * scalar; ret[10] = mat[10] * scalar; ret[14] = mat[14] * scalar;
        //    ret[3] = mat[3] * scalar; ret[7] = mat[7] * scalar; ret[11] = mat[11] * scalar; ret[15] = mat[15] * scalar;
        //    return ret;
        //}

        ///**
        // * Overloading the addition operator
        // */
        //public static Matrix operator +(Matrix left, Matrix right)
        //{
        //    Matrix ret = new Matrix();

        //    ret[0] = left[0] + right[0]; ret[4] = left[4] + right[4]; ret[8] = left[8] + right[8]; ret[12] = left[12] + right[12];
        //    ret[1] = left[1] + right[1]; ret[5] = left[5] + right[5]; ret[9] = left[9] + right[9]; ret[13] = left[13] + right[13];
        //    ret[2] = left[2] + right[2]; ret[6] = left[6] + right[6]; ret[10] = left[10] + right[10]; ret[14] = left[14] + right[14];
        //    ret[3] = left[3] + right[3]; ret[7] = left[7] + right[7]; ret[11] = left[11] + right[11]; ret[15] = left[15] + right[15];

        //    return ret;
        //}

        //#endregion

        //#region Methods

        ///**
        //* Load identity matrix
        //*/
        //public void loadIdentity()
        //{
        //    values[0] = 1.0; values[4] = 0.0; values[8] = 0.0; values[12] = 0.0;
        //    values[1] = 0.0; values[5] = 1.0; values[9] = 0.0; values[13] = 0.0;
        //    values[2] = 0.0; values[6] = 0.0; values[10] = 1.0; values[14] = 0.0;
        //    values[3] = 0.0; values[7] = 0.0; values[11] = 0.0; values[15] = 1.0;
        //}

        ///**
        // * Return a transposed matrix of this
        // */
        //public Matrix getTransposed()
        //{
        //    Matrix ret = new Matrix();
        //    ret[0] = values[0]; ret[4] = values[1]; ret[8] = values[2]; ret[12] = values[3];
        //    ret[1] = values[4]; ret[5] = values[5]; ret[9] = values[6]; ret[13] = values[7];
        //    ret[2] = values[8]; ret[6] = values[9]; ret[10] = values[10]; ret[14] = values[11];
        //    ret[3] = values[12]; ret[7] = values[13]; ret[11] = values[14]; ret[15] = values[15];
        //    return ret;
        //}

        //#endregion
    }
}
