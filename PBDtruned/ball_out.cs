using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball_out : MonoBehaviour
{
    public GameObject Ball;
    void Start()
    {
        for (int i = 0; i < 10; i++)
            Instantiate(Ball, new Vector3(i * 2f, 8, 0), new Quaternion(0, 90, 0, 0));
    }
    void Update()
    {

    }
}
