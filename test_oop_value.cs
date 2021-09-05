using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class test_oop_value : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

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

        public AbstractConstraint() { }
        public virtual double calculateValue()
        {
            return 0;//翻譯constraint.hpp 第35
        }
        public virtual double[] calculateGrad(double[] grad_C)
        {
            return grad_C; //翻譯constraint.hpp 第42
        }
        double m_stiffness;
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
            for (int j = 0; j < 4; ++j)
            {
                //m_particle要設定好
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



