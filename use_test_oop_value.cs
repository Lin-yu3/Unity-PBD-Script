using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using position_based_dynamics;
public class use_test_oop_value : MonoBehaviour
{
    // Start is called before the first frame update
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

        if (float.IsNaN(n_0.x) == true) print("ERROR!!! n_0的x值是NAN");
        if (float.IsNaN(n_0.y) == true) print("ERROR!!! n_0的y值是NAN");
        if (float.IsNaN(n_0.z) == true) print("ERROR!!! n_0的z值是NAN");
        if (float.IsNaN(n_1.x) == true) print("ERROR!!! n_1的x值是NAN");
        if (float.IsNaN(n_1.y) == true) print("ERROR!!! n_1的y值是NAN");
        if (float.IsNaN(n_1.z) == true) print("ERROR!!! n_1的z值是NAN");

        float angle = Vector3.Dot(n_0, n_1);
        if (angle < -1) angle = -1;
        else if (angle > 1) angle = 1;
        float dihedral_angle = Mathf.Acos(angle);
        if (!float.IsNaN(dihedral_angle)) Console.WriteLine("dihedral_angle不是NAN");

        var constraint = new BendingConstraint(p_0, p_1, p_2, p_3, 1.0, 0.0, dt, dihedral_angle);
        double value = constraint.calculateValue();
        double[] grad = new double[12];
        constraint.calculateGrad(grad);

        double epsilon = 1e-20;
        if (Math.Abs(value) < epsilon == true) print("value的確是很小的數");
        if (Accord.Math.Norm.Euclidean(grad) < epsilon == true) print("grad的確是很小的數");
    }
    // Update is called once per frame
    void Update()
    {

    }


}




