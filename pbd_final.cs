using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pbd_final : MonoBehaviour
{
    const int N = 8;//可自行設定有幾顆球
    public GameObject[] sphere = new GameObject[N];
    Vector3[] gradient = new Vector3[N];//之後會由 posAD 的項,算出對應的 gradient
    Vector3[] v = new Vector3[N];//初始速度v
    Vector3[] pos = new Vector3[N];//論文中的p
    Vector3[] pos0 = new Vector3[N];//論文中的x
    Vector3[] C = new Vector3[N];
    int frame = 0;
    void Start()
    {
        for (int i = 0; i < N; i++)
        {
            sphere[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere[i].transform.position = new Vector3(i * 1.5f, 0, 0);
            gradient[i] = new Vector3();
            v[i] = new Vector3();
            pos[i] = sphere[i].transform.position;
            pos0[i] = sphere[i].transform.position;
        }
        genStretchBendingConstraint();
    }

    void Update()
    {
        frame++;//因為processing每次30frame, Unity每次60frame
        if (frame % 2 == 0) return;
        solver();
        for (int i = 1; i < N; i++)
        {
            sphere[i].transform.position = pos0[i];
        }
    }
    float d0 = 1.5f;
    //float angle0 = 0;
    int[,] a;
    int[,] b;
    void genStretchBendingConstraint()
    {
        a = new int[N - 1, N];
        for (int i = 0; i < N - 1; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (i == j) a[i, j] = -1;
                else if (i + 1 == j) a[i, j] = 1;
                else a[i, j] = 0;
            }
        }
        b = new int[N - 2, 4];
        for (int i = 0; i < N - 2; i++)
        {
            b[i, 0] = i + 1;
            b[i, 1] = i;
            b[i, 2] = i + 2;
            b[i, 3] = i + 1;
        }
    }
    dfloat CjBend(int Ci, Vector3[] pos)
    {
        dfloat[] posAD = new dfloat[N * 3];
        for (int pi = 0; pi < N; pi++)
        {
            posAD[pi * 3 + 0] = new dfloat(N * 3, pos[pi].x);
            posAD[pi * 3 + 1] = new dfloat(N * 3, pos[pi].y);
            posAD[pi * 3 + 2] = new dfloat(N * 3, pos[pi].z);
            posAD[pi * 3 + 0].val(pi * 3 + 0 + 1) = 1;
            posAD[pi * 3 + 1].val(pi * 3 + 1 + 1) = 1;
            posAD[pi * 3 + 2].val(pi * 3 + 2 + 1) = 1;
        }
        //Vector3 v1 = pos[ b[Ci,0] ] - pos[ b[Ci,1] ];
        //Vector3 v2 = pos[ b[Ci,2] ] - pos[ b[Ci,3] ];
        //return 0.01f*(Vector3.Angle(v1,v2)-angle0);
        dfloat v1x = posAD[b[Ci, 0] * 3 + 0] - posAD[b[Ci, 1] * 3 + 0];
        dfloat v1y = posAD[b[Ci, 0] * 3 + 1] - posAD[b[Ci, 1] * 3 + 1];
        dfloat v1z = posAD[b[Ci, 0] * 3 + 2] - posAD[b[Ci, 1] * 3 + 2];
        dfloat v2x = posAD[b[Ci, 2] * 3 + 0] - posAD[b[Ci, 3] * 3 + 0];
        dfloat v2y = posAD[b[Ci, 2] * 3 + 1] - posAD[b[Ci, 3] * 3 + 1];
        dfloat v2z = posAD[b[Ci, 2] * 3 + 2] - posAD[b[Ci, 3] * 3 + 2];
        dfloat dot = v1x * v2x + v1y * v2y + v1z * v2z;
        dfloat len1 = dfloat.dsqrt(v1x * v1x + v1y * v1y + v1z * v1z);
        dfloat len2 = dfloat.dsqrt(v2x * v2x + v2y * v2y + v2z * v2z);
        return dfloat.dacos(dot / len1 / len2);
    }
    dfloat CjStretch(int Ci, Vector3[] pos)
    {
        dfloat[] posAD = new dfloat[N * 3];
        for (int pi = 0; pi < N; pi++)
        {
            posAD[pi * 3 + 0] = new dfloat(N * 3, pos[pi].x);
            posAD[pi * 3 + 1] = new dfloat(N * 3, pos[pi].y);
            posAD[pi * 3 + 2] = new dfloat(N * 3, pos[pi].z);
            posAD[pi * 3 + 0].val(pi * 3 + 0 + 1) = 1;
            posAD[pi * 3 + 1].val(pi * 3 + 1 + 1) = 1;
            posAD[pi * 3 + 2].val(pi * 3 + 2 + 1) = 1;
        }
        dfloat dx = new dfloat(N * 3, 0);
        dfloat dy = new dfloat(N * 3, 0);
        dfloat dz = new dfloat(N * 3, 0);
        for (int Pj = 0; Pj < N; Pj++)
        {
            float w = a[Ci, Pj];
            dx += posAD[Pj * 3 + 0] * w;
            dy += posAD[Pj * 3 + 1] * w;
            dz += posAD[Pj * 3 + 2] * w;
        }
        return dfloat.dsqrt(dx * dx + dy * dy + dz * dz) - d0;
    }
    dfloat calcCj(int j)
    {
        if (j < N - 1) return CjStretch(j, pos);
        else return CjBend(j - (N - 1), pos);
    }
    void projectConstraints()//solver()會呼叫
    {
        float maxMag = 0;
        float[] w = new float[N];
        for (int i = 0; i < N; i++) w[i] = 1.0f / N;
        for (int j = 0; j < (N - 1) + (N - 2); j++)
        {
            dfloat gC = calcCj(j);//input:第Cj條constraint, output: gradient[8]共24個變數
            float gradientSum = 0;
            for (int Xi = 1; Xi < N; Xi++)
            {//可能有個小 bug 關於 Xi=0 的最左邊的點,為什麼不動? 可設成 限制條件啊!
                float dx = gC.val(Xi * 3), dy = gC.val(Xi * 3 + 1), dz = gC.val(Xi * 3 + 2);
                gradientSum += w[Xi] * (dx * dx + dy * dy + dz * dz);
            }
            if (float.IsNaN(gradientSum)) continue;//遇到NAN,跳掉

            float lambda = gC.val(0) / gradientSum;
            for (int Xi = 1; Xi < N; Xi++)
            {  //更新, Xi=0那端釘在牆上, 有更好的寫法嗎?
                gradient[Xi].x = gC.val(Xi * 3 + 0 + 1);
                gradient[Xi].y = gC.val(Xi * 3 + 1 + 1);
                gradient[Xi].z = gC.val(Xi * 3 + 2 + 1);
                Vector3 diff = gradient[Xi] * -lambda * w[Xi];
                pos[Xi] += diff * (1 - stiffness);
                if (diff.magnitude > maxMag) maxMag = diff.magnitude;
            }
        }
    }
    int solverIterators = 1;// Iteration越多次,彈簧感覺越硬
    float stiffness = 0.9f;// (1-stiffness)的值越小,彈簧感覺越軟 
    void solver()
    {
        Vector3 g = new Vector3(0, -0.98f / 20, 0);
        for (int i = 1; i < N; i++)
        {
            v[i] += g;//Step (5) 還沒有乘上 delta T 及重量
            pos[i] = pos0[i] + v[i];//Step (7)
            v[i] *= 0.9f;//增加較大的摩擦力,系統較快能停下來
        }
        for (int i = 0; i < solverIterators; i++)//step (9)
        {
            projectConstraints();//step (10)
        }//step (11) End loop
        for (int i = 1; i < N; i++)//step (12)
        {
            v[i] = pos[i] - pos0[i];//Step (13)
            pos0[i] = pos[i];//Step (14)
        }//step (15)
        //step (16) UpdateVelocities()
    }
}