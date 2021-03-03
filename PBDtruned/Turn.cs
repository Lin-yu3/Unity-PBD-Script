using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour
{
    public GameObject center; //以誰為中心進行公轉
    public float RotationSpeed; //自轉的速度
    //public float RevolutionSpeed; //公轉速度
    void Start()
    {

    }
    void Update()
    {

        transform.Rotate(Vector3.down * RotationSpeed, Space.World); //物體自轉
        //this.transform.RotateAround(center.transform.position, Vector3.up, RevolutionSpeed);//設定公轉
    }
}
