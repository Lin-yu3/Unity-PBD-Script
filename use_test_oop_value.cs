using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using position_based_dynamics;
public class use_test_oop_value : MonoBehaviour
{
    // Start is called before the first frame update

    // public GameObject p_0, p_1, p_2, p_3;
    void Start()
    {
        double dt = 1 / 60;

        var p_0 = new Vector3(0, 0, 0);
        var p_1 = new Vector3(0, 1, 0);
        var p_2 = new Vector3(-0.5f, 0.5f, 0);
        var p_3 = new Vector3(+0.5f, 0.5f, 0);

        Vector3 x_0 = p_0;
        Vector3 x_1 = p_1;
        Vector3 x_2 = p_2;
        Vector3 x_3 = p_3;

        Vector3 p_10 = x_1 - x_0;
        Vector3 p_20 = x_2 - x_0;
        Vector3 p_30 = x_3 - x_0;

        Vector3 n_0 = Vector3.Cross(p_10, p_20).normalized;
        Vector3 n_1 = Vector3.Cross(p_10, p_30).normalized;

        float angle = Vector3.Dot(n_0, n_1);
        if (angle < -1) angle = -1;
        else if (angle > 1) angle = 1;
        float dihedral_angle = Mathf.Acos(angle);
        if (!float.IsNaN(dihedral_angle)) Console.WriteLine("dihedral_angle不是NAN");

        var constraint = new BendingConstraint(p_0, p_1, p_2, p_3, 1.0, 0.0, dt, dihedral_angle);
        // double value= calculateValue();
    }
    // Update is called once per frame
    void Update()
    {

    }


}




