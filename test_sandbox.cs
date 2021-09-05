using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Accord.Math;
public class test_sandbox : MonoBehaviour
{
    //測試


    // VectorXd
    public double[] VectorXd(double[] input)
    {
        double[] output = new double[input.GetLength(0)];
        for (int i = 0; i < input.GetLength(0); i++)
        {
            output[i] += input[i];
        }
        return output;
    }
    // void print(double[,] ans)
    // {
    //     var line = "";
    //     for (int i = 0; i < ans.GetLength(0); i++)
    //     {
    //         for (int j = 0; j < ans.GetLength(1); j++)
    //         {
    //             line += ans[i, j] + " ";
    //         }
    //         line += "\n";
    //     }
    //     print(line);
    // }
    public virtual double calculateValue()
    {
        return 0;//翻譯constraint.hpp 第35
    }
    void Start()
    {
        double[] grad_C = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        double[] m_inv_M = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        //double C = calculateValue();
        double sum = 0;
        for (int i = 0; i < 12; i++)
        {
            sum += grad_C[i] * m_inv_M[i] * grad_C[i];
            print("sum:　" + sum);
        }

        //         double[,] convertVecToCrossOp(Vector3 vec)
        //         {
        //             double[,] mat = new double[3, 3];
        //             mat[0, 1] = -vec[2];
        //             mat[0, 2] = +vec[1];
        //             mat[1, 0] = +vec[2];
        //             mat[1, 2] = -vec[0];
        //             mat[2, 0] = -vec[1];
        //             mat[2, 1] = +vec[0];
        //             return mat;
        //         }
        //         //作轉換, 準備給Accord
        //         double[] ToAccord(Vector3 s)
        //         {
        //             double[] ans = new double[] { s.x, s.y, s.z };
        //             return ans;
        //         }
        //         //Matrix 3*3
        //         double[,] Matrix3x3(Vector3 a)
        //         {
        //             double[,] vecToMatrix3x3 = { { a.x, a.y, a.z } };
        //             return vecToMatrix3x3;
        //         }

        //         Func<Vector3, Vector3, Vector3, double[,]> calc_grad_of_normalized_cross_prod_wrt_p_a = (Vector3 p_a, Vector3 p_b, Vector3 n) =>
        //              {
        //                  double left = (1 / Vector3.Cross(p_a, p_b).magnitude);
        //                  double[,] neg_con_p_b = Accord.Math.Elementwise.Multiply(-1, convertVecToCrossOp(p_b));
        //                  double[,] n_mult_n_cross_p_b_T = Accord.Math.Matrix.TransposeAndDot(Matrix3x3(n), Matrix3x3(Vector3.Cross(n, p_a)));
        //                  double[,] right = Accord.Math.Elementwise.Add(neg_con_p_b, n_mult_n_cross_p_b_T);
        //                  return Accord.Math.Elementwise.Multiply(left, right);
        //              };
        //         Vector3 p_1 = new Vector3(1, 3, 5);
        //         Vector3 p_2 = new Vector3(2, 4, 6);
        //         Vector3 n_0 = new Vector3(7, 8, 9);
        //         //測試函式呼叫函式
        //         double[,] partial_n_0_per_partial_p_1 = calc_grad_of_normalized_cross_prod_wrt_p_a(p_1, p_2, n_0);
        //         print(partial_n_0_per_partial_p_1);

        //         // Creates a vector of indices
        //         int[] idx = Accord.Math.Vector.Range(0, 10);  // { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
        //         // Creates a step vector within a given interval
        //         int[] interval = Accord.Math.Vector.Interval(-2, 4); // { -2, -1, 0, 1, 2, 3, 4 };
        //         // Special matrices
        //         double[,] I = Accord.Math.Matrix.Identity(3);     // creates a 3x3 identity matrix
        //         double[,] magic = Accord.Math.Matrix.Magic(5);    // creates a magic square matrix of size 5
        //         double[] vv = Accord.Math.Vector.Create(5, 1.0);      // generates { 1, 1, 1, 1, 1 }
        //         double[,] diagonal = Accord.Math.Matrix.Diagonal(vv); // matrix with v on its diagonal
        //         //測試點乘 
        //         double[] vector = { 0, 2, 4 };
        //         double[] test_a = Accord.Math.Elementwise.Multiply(2, vector); // vector .* 2, generates { 0,  4,  8 }
        //         double[] test_b = Accord.Math.Elementwise.Divide(vector, 2);   // vector ./ 2, generates { 0,  1,  2 }
        //         double[] c = Accord.Math.Elementwise.Pow(vector, 2);
        //         // Declare two vectors
        //         double[] u = { 1, 6, 3 };
        //         double[] v = { 9, 4, 2 };

        //         // Products between vectors
        //         double inner = Accord.Math.Matrix.Dot(u, v);    // 39.0
        //         double[,] outer = Accord.Math.Matrix.Outer(u, v); // see below
        //         double[] kronecker = Accord.Math.Matrix.Kronecker(u, v); // { 9, 4, 2, 54, 24, 12, 27, 12, 6 }
        //                                                                  //double[][] cartesian = Accord.Math.Matrix.Cartesian<cartesian>(u,v); // all possible pair-wise combinations

        //         /* outer =
        //            { 
        //               {  9,  4,  2 },
        //               { 54, 24, 12 },
        //               { 27, 12,  6 },
        //            };                  */

        //         // Addition
        //         double[] addv = Accord.Math.Elementwise.Add(u, v); // { 10, 10, 5 }
        //         double[] add5 = Accord.Math.Elementwise.Add(u, 5); // {  6, 11, 8 }
        //         // // Elementwise operations
        //         double[] abs = Accord.Math.Elementwise.Abs(u);   // { 1, 6, 3 }
        //         double[] log = Accord.Math.Elementwise.Log(u);   // { 0, 1.79, 1.09 }
        //         // Apply *any* function to all elements in a vector
        //         double[] cos = Accord.Math.Matrix.Apply(u, Math.Cos); // { 0.54, 0.96, -0.989 }
        //         Accord.Math.Matrix.Apply(u, Math.Cos); // can also do optionally in-place


        //         // Declare a matrix
        //         double[,] M =
        //         {
        //      { 0, 5, 2 },
        //      { 2, 1, 5 }
        //   };

        //         double[] min_v = { 9, 4 };
        //         // Some operations between vectors and matrices
        //         double[] Mv = Accord.Math.Matrix.Dot(M, v);    //  { 24, 32 }
        //         double[] vM = Accord.Math.Matrix.Dot(min_v, M); // { 8, 49, 38 }
        //         // Some operations between matrices
        //         double[,] Md = Accord.Math.Matrix.DotWithDiagonal(M, v);   // { { 0, 20, 4 }, { 18, 4, 10 } }
        //         double[,] MMt = Accord.Math.Matrix.DotWithTransposed(M, M); //   { { 29, 15 }, { 15, 30 } }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
