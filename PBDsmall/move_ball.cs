using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_ball : MonoBehaviour
{
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKey("down"))
        {
            transform.Translate(0, -1, 0);
        }
        if (Input.GetKey("up"))
        {
            transform.Translate(0, 1, 0);
        }

        if (Input.GetKey("left"))
        {
            transform.Translate(-1, 0, 0);
        }

        if (Input.GetKey("right"))
        {
            transform.Translate(1, 0, 0);
        }
    }
}
