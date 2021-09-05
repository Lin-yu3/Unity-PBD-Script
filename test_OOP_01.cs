using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_OOP_01 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

class BendingConstraint : AbstractConstraint
{
    public BendingConstraint(Vector3 p_0, Vector3 p_1, Vector3 p_2, Vector3 p_3)
    {

    }

}
class AbstractConstraint
{

    public AbstractConstraint()
    {

    }
    public virtual double calculateValue()
    {
        return 0;
    }
    public virtual Vector3[] calculateGrad(Vector3[] grad_C)
    {
        return grad_C;
    }
    double m_stiffness;


    double[,] grad_C_T;

    void projectParticles()
    {
        //計算constraint function的值
        double C = calculateValue();
        Vector3[,] grad_C_T = new Vector3[12, 1];
        //grad_C_T = calculateGrad();

        //grad_C 的 Transpose矩陣
        double[,] arr1 = new double[12, 1];
        double[,] arr2 = new double[1, 12];
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 1; j++)
            {
                arr2[j, i] = arr1[i, j];
            }
        }
        //計算s
        double[,] m_inv_M = new double[12, 1];
        //const double s = -C / (arr2 * m_inv_M * grad_C);
        //計算Delta x
        double[,] delta_x = new double[12, 1];
        //delta_x = m_stiffness * s * m_inv_M * grad_C;
    }

}
