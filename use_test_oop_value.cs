using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class use_test_oop_value : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameObject[] p = new GameObject[4];
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            p[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
        p[0].transform.position = new Vector3(1.5f, 3, 3);
        p[1].transform.position = new Vector3(3, 0, 1.5f);
        p[2].transform.position = new Vector3(4.5f, 0, 3);
        p[3].transform.position = new Vector3(6, 3, 1.5f);
        print("void start");
        print("p[0] :" + p[0].transform.position);
        print("p[1] :" + p[1].transform.position);
        print("p[2] :" + p[2].transform.position);
        print("p[3] :" + p[3].transform.position);


    }
    // Update is called once per frame
    void Update()
    {

    }
    class BendingConstraint : AbstractConstraint
    {
        public BendingConstraint(Vector3 p_0, Vector3 p_1, Vector3 p_2, Vector3 p_3)
        {
            //自己預設值

        }

    }
    class AbstractConstraint
    {
        public double[,] convertVecToCrossOp(Vector3 vec)
        {
            double[,] mat = new double[3, 3];
            mat[0, 1] = -vec[2];
            mat[0, 2] = +vec[1];
            mat[1, 0] = +vec[2];
            mat[1, 2] = -vec[0];
            mat[2, 0] = -vec[1];
            mat[2, 1] = +vec[0];
            return mat;
        }
        public AbstractConstraint() { }
        public virtual double calculateValue()
        {
            //翻譯constraint.cpp 第51-69
            Vector3 x_0 = p[0].transform.position;
            Vector3 x_1 = p[1].transform.position;
            Vector3 x_2 = p[2].transform.position;
            Vector3 x_3 = p[3].transform.position;

            Vector3 p_1 = x_1 - x_0;
            Vector3 p_2 = x_2 - x_0;
            Vector3 p_3 = x_3 - x_0;

            Vector3 p_1_cross_p_2 = Vector3.Cross(p_1, p_2);
            Vector3 p_1_cross_p_3 = Vector3.Cross(p_1, p_3);

            Vector3 n_0 = p_1_cross_p_2.normalized;
            Vector3 n_1 = p_1_cross_p_3.normalized;
            float n_0_dot_n_1 = Vector3.Dot(n_0, n_1);
            if (n_0_dot_n_1 < -1) return -1;
            else if (n_0_dot_n_1 > 1) return 1;
            const double m_dihedral_angle = 0;//原始角度:0 (不確定?)
            double current_dihedral_angle = Mathf.Acos(n_0_dot_n_1);
            // assert(n_0.norm() > 0.0);
            // assert(n_1.norm() > 0.0);
            // assert(!std::isnan(current_dihedral_angle));
            return current_dihedral_angle - m_dihedral_angle;

        }
        public virtual double[] calculateGrad(double[] grad_C)
        {
            //翻譯constraint.cpp 第75-
            Vector3 x_0 = p[0].transform.position;
            Vector3 x_1 = p[1].transform.position;
            Vector3 x_2 = p[2].transform.position;
            Vector3 x_3 = p[3].transform.position;

            Vector3 p_1 = x_1 - x_0;
            Vector3 p_2 = x_2 - x_0;
            Vector3 p_3 = x_3 - x_0;

            Vector3 p_1_cross_p_2 = Vector3.Cross(p_1, p_2);
            Vector3 p_1_cross_p_3 = Vector3.Cross(p_1, p_3);

            Vector3 n_0 = p_1_cross_p_2.normalized;
            Vector3 n_1 = p_1_cross_p_3.normalized;
            float d = Vector3.Dot(n_0, n_1);

            double epsilon = 1e-12;
            if (1 - d * d < epsilon)
            {
                for (int i = 0; i < 12; i++)
                {
                    grad_C[i] = 0;
                }
                return grad_C;
            }
            double common_coeff = -1 / Mathf.Sqrt(1 - d * d);

            //作轉換, 準備給Accord
            double[] ToAccord(Vector3 v)
            {
                double[] ans = new double[] { v.x, v.y, v.z };
                for (int i = 0; i < ans.GetLength(0); i++)
                {
                    print("mat" + "[0 ," + i + "] :" + ans[i]);
                }
                return ans;
            }

            //Matrix 3*3
            double[,] Matrix3x3(Vector3 a)
            {
                double[,] vecToMatrix3x3 = { { a.x, a.y, a.z } };
                return vecToMatrix3x3;
            }
            Func<Vector3, Vector3, Vector3, double[,]> calc_grad_of_normailzed_cross_prod_wrt_p_a = (Vector3 p_a, Vector3 p_b, Vector3 n) =>
            {
                double left = 1 / Vector3.Cross(p_a, p_b).magnitude;
                double[,] neg_con_p_b = Accord.Math.Elementwise.Multiply(-1, convertVecToCrossOp(p_b));
                double[,] n_mult_n_cross_p_b_T = Accord.Math.Matrix.TransposeAndDot(Matrix3x3(n), Matrix3x3(Vector3.Cross(n, p_b)));
                double[,] right = Accord.Math.Elementwise.Add(neg_con_p_b, n_mult_n_cross_p_b_T);
                return Accord.Math.Elementwise.Multiply(left, right);
            };
            Func<Vector3, Vector3, Vector3, double[,]> calc_grad_of_normailzed_cross_prod_wrt_p_b = (Vector3 p_a, Vector3 p_b, Vector3 n) =>
            {
                double left = -(1 / Vector3.Cross(p_a, p_b).magnitude);
                double[,] neg_con_p_a = Accord.Math.Elementwise.Multiply(-1, convertVecToCrossOp(p_a));
                double[,] n_mult_n_cross_p_a_T = Accord.Math.Matrix.TransposeAndDot(Matrix3x3(n), Matrix3x3(Vector3.Cross(n, p_a)));
                double[,] right = Accord.Math.Elementwise.Add(neg_con_p_a, n_mult_n_cross_p_a_T);
                return Accord.Math.Elementwise.Multiply(left, right);
            };
            double[,] partial_n_0_per_partial_p_1 = calc_grad_of_normailzed_cross_prod_wrt_p_a(p_1, p_2, n_0);
            double[,] partial_n_1_per_partial_p_1 = calc_grad_of_normailzed_cross_prod_wrt_p_a(p_1, p_3, n_1);
            double[,] partial_n_0_per_partial_p_2 = calc_grad_of_normailzed_cross_prod_wrt_p_b(p_1, p_2, n_0);
            double[,] partial_n_1_per_partial_p_3 = calc_grad_of_normailzed_cross_prod_wrt_p_b(p_1, p_3, n_1);

            double[] grad_C_wrt_p_1 = Accord.Math.Elementwise.Multiply(common_coeff,
                Accord.Math.Elementwise.Add(Accord.Math.Matrix.TransposeAndDot(partial_n_0_per_partial_p_1, ToAccord(n_1)),
                Accord.Math.Matrix.TransposeAndDot(partial_n_0_per_partial_p_1, ToAccord(n_0))));
            double[] grad_C_wrt_p_2 = Accord.Math.Elementwise.Multiply(common_coeff,
                Accord.Math.Matrix.TransposeAndDot(partial_n_0_per_partial_p_2, ToAccord(n_1)));
            double[] grad_C_wrt_p_3 = Accord.Math.Elementwise.Multiply(common_coeff,
                Accord.Math.Matrix.TransposeAndDot(partial_n_1_per_partial_p_3, ToAccord(n_0)));

            double[] neg_grad_C_wrt_p_1 = Accord.Math.Elementwise.Multiply(-1, grad_C_wrt_p_1);
            double[] neg_grad_C_wrt_p_2 = Accord.Math.Elementwise.Multiply(-1, grad_C_wrt_p_2);
            double[] neg_grad_C_wrt_p_3 = Accord.Math.Elementwise.Multiply(-1, grad_C_wrt_p_3);
            double[] grad_C_wrt_p_1_add_p_2 = Accord.Math.Elementwise.Add(neg_grad_C_wrt_p_1, neg_grad_C_wrt_p_2);
            double[] grad_C_wrt_p_0 = Accord.Math.Elementwise.Add(grad_C_wrt_p_1_add_p_2, neg_grad_C_wrt_p_3);
            for (int i = 0; i < 3; i++)
            {
                grad_C[i + 3 * 0] = grad_C_wrt_p_0[i];
                grad_C[i + 3 * 1] = grad_C_wrt_p_1[i];
                grad_C[i + 3 * 2] = grad_C_wrt_p_2[i];
                grad_C[i + 3 * 3] = grad_C_wrt_p_3[i];
            }
            return grad_C;
        }
    }



}




