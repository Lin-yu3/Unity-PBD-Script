using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pbd02_twospring : MonoBehaviour
{
    float x1 = -10f, y1 = 0, z1 = 0;
    float x2 = 0f, y2 = 0, z2 = 0;
    float x3 = +10f, y3 = 0, z3 = 0; ///總共有9個控制變數, 下面 dfloat 會存「對9個控制變數」微分的值
    public GameObject ball01, ball02, ball03;
    // Start is called before the first frame update
    void Start()
    {
        //https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html
        ball01 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball01.transform.position = new Vector3(x1, y1, z1);
        ball01.transform.SetParent(this.transform);
        ball02 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball02.transform.position = new Vector3(x2, y2, z2);
        ball02.transform.SetParent(this.transform);
        ball03 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball03.transform.position = new Vector3(x3, y3, z3);
        ball03.transform.SetParent(this.transform);
        //dfloat a = new dfloat(30);//原來另外一個程式裡寫的 class dfloat 這裡可以用到
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v1 = ball01.transform.position;
        Vector3 v2 = ball02.transform.position;
        Vector3 v3 = ball03.transform.position;

        x1 = v1.x; y1 = v1.y; z1 = v1.z; //設好控制變數
        x2 = v2.x; y2 = v2.y; z2 = v2.z; //設好控制變數
        x3 = v3.x; y3 = v3.y; z3 = v3.z; //設好控制變數
        solvePBD();
        ball01.transform.position = new Vector3(x1, y1, z1); //更新回去
        ball02.transform.position = new Vector3(x2, y2, z2); //更新回去
        ball03.transform.position = new Vector3(x3, y3, z3); //更新回去
    }
    void solvePBD()
    {
        float len0 = 10f; ///原始長度為 0.5
        dfloat dx1 = new dfloat(9, x1);
        dfloat dy1 = new dfloat(9, y1);
        dfloat dz1 = new dfloat(9, z1); ///初始原值, 其他裡面都會是0
        dfloat dx2 = new dfloat(9, x2);
        dfloat dy2 = new dfloat(9, y2);
        dfloat dz2 = new dfloat(9, z2); ///初始原值, 其他裡面都會是0
        dfloat dx3 = new dfloat(9, x3);
        dfloat dy3 = new dfloat(9, y3);
        dfloat dz3 = new dfloat(9, z3); ///初始原值, 其他裡面都會是0
        dx1.val(1) = 1; ///[1]項 是對 變數1 微分
        dy1.val(2) = 1; ///[2]項 是對 變數2 微分
        dz1.val(3) = 1; ///[3]項 是對 變數3 微分
        dx2.val(4) = 1; ///[4]項 是對 變數4 微分
        dy2.val(5) = 1; ///[5]項 是對 變數5 微分
        dz2.val(6) = 1; ///[6]項 是對 變數6 微分
        dx3.val(7) = 1; ///[7]項 是對 變數7 微分
        dy3.val(8) = 1; ///[8]項 是對 變數8 微分
        dz3.val(9) = 1; ///[9]項 是對 變數9 微分
                        ///沒設定的,都會是0

                        ///這裡的 dx 是前面 x=x1-x2 去做 dfloat 計算的意思
        dfloat dx = dx1 - dx2, dy = dy1 - dy2, dz = dz1 - dz2; ///AD公式, 用來算 cost function 的輔助變數
        dfloat dlenA = dfloat.dsqrt(dx * dx + dy * dy + dz * dz);
        dfloat dxx = dx2 - dx3, dyy = dy2 - dy3, dzz = dz2 - dz3;
        dfloat dlenB = dfloat.dsqrt(dxx * dxx + dyy * dyy + dzz * dzz);

        dfloat gC = dlenA - len0; ///AD公式, cost function, gC[1]..gC[6]是gradient
        gC += dlenB - len0; ///AD公式, cost function, gC[1]..gC[6]是gradient
        //把cost function都相加
        //bending,直接拿線段的向量來做
        print(dlenA.val(0) + " " + dlenB.val(0));
        gC += dfloat.dacos((dx * dxx + dy * dyy + dz * dzz) / dlenA / dlenB);
        print("acos's input val[0]:" + ((dx * dxx + dy * dyy + dz * dzz) / dlenA / dlenB).val(0));

        float len2 = 0; ///posBasedDyn.pdf 的公式(5)
        for (int i = 1; i <= 9; i++)
        {
            len2 += gC.val(i) * gC.val(i); ///要算出分母 (gradient的長度平方)
        }
        len2 = Mathf.Sqrt(len2);

        print(gC.val(0) + " " + gC.val(1) + " " + gC.val(2) + " " + gC.val(3));
        float C = gC.val(0); //其實 cost function C的, 就存在 gC 的第[0]項
        x1 += (-C / len2) * gC.val(1); ///△P=-C(P)/|▽pC(P)|的平方 × ▽pC(P)
        y1 += (-C / len2) * gC.val(2); ///算出 △P 回去改 P
        z1 += (-C / len2) * gC.val(3);
        x2 += (-C / len2) * gC.val(4);
        y2 += (-C / len2) * gC.val(5);
        z2 += (-C / len2) * gC.val(6);
        x3 += (-C / len2) * gC.val(7);
        y3 += (-C / len2) * gC.val(8);
        z3 += (-C / len2) * gC.val(9);
    }
}
