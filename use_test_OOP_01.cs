using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class use_test_OOP_01 : MonoBehaviour
{
    GameObject[] sphere = new GameObject[4];

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            sphere[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
        sphere[0].transform.position = new Vector3(1.5f, 1.5f, 0);
        sphere[1].transform.position = new Vector3(3, 0, 0);
        sphere[2].transform.position = new Vector3(4.5f, 1.5f, 0);
        sphere[3].transform.position = new Vector3(6, 1.5f, 0);

    }
    void Update()
    {

    }

    //翻譯Constraint.cpp
    /*BendingConstraint:calculateValue()
    {
        Vector3 x_0 = sphere[0].transform.position;
        Vector3 x_1 = sphere[1].transform.position;
        Vector3 x_2 = sphere[2].transform.position;
        Vector3 x_3 = sphere[3].transform.position;
        Vector3 p_10 = x_1 - x_0;
        Vector3 p_20 = x_2 - x_0;
        Vector3 p_30 = x_3 - x_0;

        Vector3 n_0 = Vector3.Cross(p_10, p_20).normalized;
        Vector3 n_1 = Vector3.Cross(p_10, p_30).normalized;

        double current_dihedral_angle = Mathf.Acos(Vector3.Dot(n_1, n_0));
    }
    //計算gradient
    BendingConstraint:calculateGrad(Vector3[] grad_C)
    {
        Vector3 x_0 = sphere[0].transform.position;
        Vector3 x_1 = sphere[1].transform.position;
        Vector3 x_2 = sphere[2].transform.position;
        Vector3 x_3 = sphere[3].transform.position;
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
            //std::fill(grad_C, grad_C + 12, 0.0);
            return;
        }
        float common_coeff = -1 / Mathf.Sqrt(1 - d * d);

    }*/



}
