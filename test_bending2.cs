using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_bending2 : MonoBehaviour
{
    static int N = 16;
    public GameObject[] sphere = new GameObject[N];
    public Vector3[] P = new Vector3[N];
    float calcBend(int i)
    {
        if (i > N - 3) return -1;//超過最後一顆球

        Vector3 v1 = P[1 + i] - P[0 + i], v2 = P[2 + i] - P[0 + i], v3 = P[3 + i] - P[0 + i];
        Vector3 n1 = Vector3.Cross(v1, v2), n2 = Vector3.Cross(v1, v3);
        n1.Normalize(); n2.Normalize();
        return Mathf.Acos(Vector3.Dot(n1, n2)) * 180 / Mathf.PI;

    }
    void Start()
    {
        for (int i = 0; i < N; i++)
        {
            sphere[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            P[i] = new Vector3(i - 2, 0, i % 2 - 0.5f);//把座標變成閃電
            sphere[i].transform.position = P[i];
        }
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = N;
        lr.SetPositions(P);

        for (int i = 0; i < N - 3; i++)
        {
            print(i + ":" + calcBend(i));
        }
    }
    void Update()
    {

    }
}
