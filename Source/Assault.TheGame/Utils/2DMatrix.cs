﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Utils
{
  public class C2DMatrix
  {
    class Matrix
    {
      public double _11, _12, _13;
      public double _21, _22, _23;
      public double _31, _32, _33;

      public Matrix()
      {
        _11 = 0.0; _12 = 0.0; _13 = 0.0;
        _21 = 0.0; _22 = 0.0; _23 = 0.0;
        _31 = 0.0; _32 = 0.0; _33 = 0.0;
      }
    };

    //accessors to the matrix elements
    double _11 { get { return m_Matrix._11; } set { m_Matrix._11 = value; } }
    double _12 { get { return m_Matrix._12; } set { m_Matrix._12 = value; } }
    double _13 { get { return m_Matrix._13; } set { m_Matrix._13 = value; } }

    double _21 { get { return m_Matrix._21; } set { m_Matrix._21 = value; } }
    double _22 { get { return m_Matrix._22; } set { m_Matrix._22 = value; } }
    double _23 { get { return m_Matrix._23; } set { m_Matrix._23 = value; } }

    double _31 { get { return m_Matrix._31; } set { m_Matrix._31 = value; } }
    double _32 { get { return m_Matrix._32; } set { m_Matrix._32 = value; } }
    double _33 { get { return m_Matrix._33; } set { m_Matrix._33 = value; } }

    Matrix m_Matrix = new Matrix();
    
    public C2DMatrix()
    {
      Identity();
    }    

    private void MatrixMultiply(Matrix mIn)
    {
      Matrix mat_temp = new Matrix();

      //first row
      mat_temp._11 = (m_Matrix._11 * mIn._11) + (m_Matrix._12 * mIn._21) + (m_Matrix._13 * mIn._31);
      mat_temp._12 = (m_Matrix._11 * mIn._12) + (m_Matrix._12 * mIn._22) + (m_Matrix._13 * mIn._32);
      mat_temp._13 = (m_Matrix._11 * mIn._13) + (m_Matrix._12 * mIn._23) + (m_Matrix._13 * mIn._33);

      //second
      mat_temp._21 = (m_Matrix._21 * mIn._11) + (m_Matrix._22 * mIn._21) + (m_Matrix._23 * mIn._31);
      mat_temp._22 = (m_Matrix._21 * mIn._12) + (m_Matrix._22 * mIn._22) + (m_Matrix._23 * mIn._32);
      mat_temp._23 = (m_Matrix._21 * mIn._13) + (m_Matrix._22 * mIn._23) + (m_Matrix._23 * mIn._33);

      //third
      mat_temp._31 = (m_Matrix._31 * mIn._11) + (m_Matrix._32 * mIn._21) + (m_Matrix._33 * mIn._31);
      mat_temp._32 = (m_Matrix._31 * mIn._12) + (m_Matrix._32 * mIn._22) + (m_Matrix._33 * mIn._32);
      mat_temp._33 = (m_Matrix._31 * mIn._13) + (m_Matrix._32 * mIn._23) + (m_Matrix._33 * mIn._33);

      m_Matrix = mat_temp;
    }

    ////applies a 2D transformation matrix to a std::vector of Vector2Ds
    public void TransformVector2Ds(List<Vector2> points)
    {
      for (int i=0; i<points.Count; ++i)
      {
        double tempX =(m_Matrix._11*points[i].X) + (m_Matrix._21*points[i].Y) + (m_Matrix._31);
        double tempY = (m_Matrix._12*points[i].X) + (m_Matrix._22*points[i].Y) + (m_Matrix._32);

        points[i]= new Vector2((float)tempX, (float)tempY);
      }
    }

    ////applies a 2D transformation matrix to a single Vector2D
    public void TransformVector2Ds(Vector2 vPoint)
    {
      double tempX =(m_Matrix._11*vPoint.X) + (m_Matrix._21*vPoint.Y) + (m_Matrix._31);
      double tempY = (m_Matrix._12*vPoint.X) + (m_Matrix._22*vPoint.Y) + (m_Matrix._32);

      vPoint.X = (float)tempX;
      vPoint.Y = (float)tempY;
    }

    ////create an identity matrix
    void Identity()
    {
      m_Matrix._11 = 1; m_Matrix._12 = 0; m_Matrix._13 = 0;
      m_Matrix._21 = 0; m_Matrix._22 = 1; m_Matrix._23 = 0;
      m_Matrix._31 = 0; m_Matrix._32 = 0; m_Matrix._33 = 1;
    }

    ////create a transformation matrix
    public void Translate(double x, double y)
    {
      Matrix mat = new Matrix();

      mat._11 = 1; mat._12 = 0; mat._13 = 0;

      mat._21 = 0; mat._22 = 1; mat._23 = 0;

      mat._31 = x;    mat._32 = y;    mat._33 = 1;

      //and multiply
      MatrixMultiply(mat);
    }

    ////create a scale matrix
    //inline void C2DMatrix::Scale(double xScale, double yScale)
    //{
    //  C2DMatrix::Matrix mat;

    //  mat._11 = xScale; mat._12 = 0; mat._13 = 0;

    //  mat._21 = 0; mat._22 = yScale; mat._23 = 0;

    //  mat._31 = 0; mat._32 = 0; mat._33 = 1;

    //  //and multiply
    //  MatrixMultiply(mat);
    //}


    ////create a rotation matrix
    public void Rotate(double rot)
    {
      Matrix mat = new Matrix();

      double Sin = Math.Sin(rot);
      double Cos = Math.Cos(rot);

      mat._11 = Cos;  mat._12 = Sin; mat._13 = 0;

      mat._21 = -Sin; mat._22 = Cos; mat._23 = 0;

      mat._31 = 0; mat._32 = 0;mat._33 = 1;

      //and multiply
      MatrixMultiply(mat);
    }


    ////create a rotation matrix from a 2D vector
    public void Rotate(Vector2 fwd, Vector2 side)
    {
      Matrix mat = new Matrix();

      mat._11 = fwd.X;  mat._12 = fwd.Y; mat._13 = 0;

      mat._21 = side.X; mat._22 = side.Y; mat._23 = 0;

      mat._31 = 0; mat._32 = 0;mat._33 = 1;

      //and multiply
      MatrixMultiply(mat);
    }
  }
}
