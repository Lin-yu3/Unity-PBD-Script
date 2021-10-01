using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace position_based_dynamics
{
    class BendingConstraint : AbstractConstraint
    {
        public BendingConstraint(Vector3 p_0,
                                 Vector3 p_1,
                                 Vector3 p_2,
                                 Vector3 p_3,
                                 double stiffness,
                                 double compliance,
                                 double delta_time,
                                 double dihedral_angle)
            : base(particles, stiffness, compliance, delta_time)
        { }
        //把原來的calculateValue複寫,蓋過去
        public override double calculateValue()
        {
            Vector3 x_0 = test_oop_value.p[0].transform.position;
            Vector3 x_1 = test_oop_value.p[1].transform.position;
            Vector3 x_2 = test_oop_value.p[2].transform.position;
            Vector3 x_3 = test_oop_value.p[3].transform.position;

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
            const double m_dihedral_angle = 0;//自己設定的(不確定能否這樣寫?)
            double current_dihedral_angle = Mathf.Acos(n_0_dot_n_1);
            if (n_0.magnitude > 0.0) Console.WriteLine("n_0長度大於0");
            if (n_1.magnitude > 0.0) Console.WriteLine("n_1長度大於0");
            if (!double.IsNaN(current_dihedral_angle)) Console.WriteLine("dihedral_angle不是NAN");
            return current_dihedral_angle - m_dihedral_angle;
        }
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
        //把原來的calculateGrad複寫,蓋過去
        public override double[] calculateGrad(double[] grad_C)
        {
            //翻譯constraint.cpp 第75-
            Vector3 x_0 = test_oop_value.p[0].transform.position;
            Vector3 x_1 = test_oop_value.p[1].transform.position;
            Vector3 x_2 = test_oop_value.p[2].transform.position;
            Vector3 x_3 = test_oop_value.p[3].transform.position;

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
                    Console.WriteLine("mat" + "[0 ," + i + "] :" + ans[i]);
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
            //翻譯constraint .cpp 第127-130
            grad_C[3 * 0 + 0] = grad_C_wrt_p_0[0];
            grad_C[3 * 0 + 1] = grad_C_wrt_p_0[1];
            grad_C[3 * 0 + 2] = grad_C_wrt_p_0[2];

            grad_C[3 * 1 + 0] = grad_C_wrt_p_1[0];
            grad_C[3 * 1 + 1] = grad_C_wrt_p_1[1];
            grad_C[3 * 1 + 2] = grad_C_wrt_p_1[2];

            grad_C[3 * 2 + 0] = grad_C_wrt_p_2[0];
            grad_C[3 * 2 + 1] = grad_C_wrt_p_2[1];
            grad_C[3 * 2 + 2] = grad_C_wrt_p_2[2];

            grad_C[3 * 3 + 0] = grad_C_wrt_p_3[0];
            grad_C[3 * 3 + 1] = grad_C_wrt_p_3[1];
            grad_C[3 * 3 + 2] = grad_C_wrt_p_3[2];
            return grad_C;
        }
        private double m_dihedral_angle;

    }
    class AbstractConstraint
    {
        public AbstractConstraint(double[] particles,
                                  double stiffness,
                                  double compliance,
                                  double delta_time)
                => (m_stiffness, m_lagrange_mutiplier, m_compliance, m_delta_time, m_particles)
                = (stiffness, 0.0, compliance, delta_time, particles);
        public double stiffness { get; }
        public double compliance { get; }
        public double delta_time { get; }
        public static double[] particles { get; }
        public double zero = 0.0;
        public void FixedNumAbstractConstraint(out double[] particles,
                                out double stiffness,
                                out double compliance,
                                out double delta_time)
                => (particles, stiffness, zero, compliance, delta_time)
                = (m_particles, m_stiffness, m_lagrange_mutiplier, m_compliance, m_delta_time);
        public virtual double calculateValue()
        {
            return 0;//翻譯constraint.hpp 第35
        }
        public virtual double[] calculateGrad(double[] grad_C)
        {
            return grad_C; //翻譯constraint.hpp 第42
        }
        double m_stiffness;
        double m_lagrange_mutiplier;
        double m_compliance;
        double m_delta_time;
        protected double[] m_particles;
        void projectParticles()
        {
            double C = calculateValue();
            double[] grad_C = calculateGrad();
            double very_small_value = 1e-12;
            if (Accord.Math.Norm.Euclidean(grad_C) < very_small_value)
            {
                return;
            }
            //計算 s
            double sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += grad_C[i] * m_inv_M[i] * grad_C[i];
            }
            double s = -C / sum;
            //計算 Delta x
            double[] delta_x = new double[12];
            double StiffMultS = m_stiffness * s;
            delta_x = Accord.Math.Elementwise.Multiply(StiffMultS, Accord.Math.Matrix.Dot(Accord.Math.Matrix.Diagonal(m_inv_M), grad_C));
            for (int i = 0; i < 12; i++)
            {
                if (double.IsNaN(delta_x[i])) continue;//如果delta x 數值非常小, 就跳掉
            }
            //更新預測的位置p
            for (int j = 0; j < 12; ++j)
            {
                m_particles[j] += delta_x[j];
            }
        }
        private double[] m_inv_M = new double[12];
        private double[] calculateGrad()
        {
            double[] grad_C = new double[12];
            calculateGrad(grad_C);
            return grad_C;
        }
        private static double[] constructIverseMassMatrix(double[] particles)
        {
            double[] inv_M = new double[12];//Num=4, 4*3=12
            for (int j = 0; j < 4; j++)
            {
                inv_M[j * 3 + 0] = particles[j];
                inv_M[j * 3 + 1] = particles[j];
                inv_M[j * 3 + 2] = particles[j];
            }
            return inv_M;
        }
    }

}
public class test_oop_value : MonoBehaviour
{
    public static GameObject[] p = new GameObject[4];
    // Start is called before the first frame update
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
    }
    // Update is called once per frame
    void Update()
    {

    }

}




