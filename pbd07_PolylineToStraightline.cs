using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pbd07_PolylineToStraightline : MonoBehaviour
{
    public int N = 15;
    float d0 = 0.8f, angle0 = Mathf.Sin(Mathf.PI);
    public GameObject[] balls;
    public GameObject[] lines;
    public Vector3[] gradient;
    public Vector3[] find_ball;
    public GameObject sugar;
    public GameObject sugar_line;
    bool bSolving = false;
    void Start()
    {
        balls = new GameObject[N];//指定陣列數量
        find_ball = new Vector3[N];
        gradient = new Vector3[N];
        for (int i = 0; i < N; i++)
        {
            GameObject throw_ball = Instantiate(sugar, new Vector3((float)i, 1, Random.Range(1f, 15f)), Quaternion.identity) as GameObject;
            throw_ball.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            balls[i] = throw_ball;
            balls[i].name = "sphere #" + i;
            find_ball[i] = balls[i].transform.position;
        }
        lines = new GameObject[N - 1];
        for (int i = 0; i < N - 1; i++)
        {
            GameObject throw_line = Instantiate(sugar_line,
            new Vector3((balls[i].transform.position.x + balls[i + 1].transform.position.x) / 2, 1, (balls[i].transform.position.z + balls[i + 1].transform.position.z) / 2),
            Quaternion.FromToRotation(Vector3.up, balls[i].transform.position - balls[i + 1].transform.position)) as GameObject;

            throw_line.transform.localScale = new Vector3(0.3f, (balls[i].transform.position - balls[i + 1].transform.position).magnitude / 2f, 0.3f);
            lines[i] = throw_line;
            lines[i].name = "line #" + i;
        }
        genStretchBendingConstraint();
    }
    void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {

            bSolving = true;

        }
        if (bSolving)
        {
            for (int i = 0; i < N; i++)
            {
                Destroy(balls[i]);
                print("delete " + balls[i]);
            }
            solver();

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector3 c = new Vector3();
            Vector3 c2 = new Vector3(N / 2, 1, N / 2);
            for (int i = 0; i < N; i++)
            {
                c += find_ball[i];
            }
            c /= N;
            for (int i = 0; i < N; i++)
            {
                find_ball[i] += -c + c2;
            }
        }

    }
    float Ci(int i, Vector3[] find_ball)
    {
        Vector3 ans = new Vector3();//預設ans=0
        for (int j = 0; j < N; j++)
        {
            float v = a[i, j];
            ans += find_ball[j] * v;//把算出來的加回去ans
        }
        //Vector3.sqrMagnitude長度平方(只是讀)
        return ans.sqrMagnitude - d0 * d0;//公式:|x1-x2|平方-d平方
    }
    public int[,] a;
    public int[,] b;
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
        b = new int[N - 2, 4];//範例{1,0,2,1}
        for (int i = 0; i < N - 2; i++)
        {
            b[i, 0] = i + 1;
            b[i, 1] = i;
            b[i, 2] = i + 2;
            b[i, 3] = i + 1;
        }
    }
    float Cbendi(int i, Vector3[] balls)
    {
        Vector3 v1 = balls[b[i, 0]] - balls[b[i, 1]];
        Vector3 v2 = balls[b[i, 2]] - balls[b[i, 3]];
        return Vector3.Angle(v1, v2) - angle0;//算出兩向量的夾角
    }
    float C(int Cj)
    {
        if (Cj < N - 1) return Ci(Cj, find_ball);
        else return Cbendi(Cj - (N - 1), find_ball);
    }

    void calcGradient(int Cj, int Xi)//input(第幾個constrain,第幾個點)
    //???地雷???
    {
        print("====Xi:" + Xi);
        float d = 0.001f;//埋炸彈
        float f0, f1;
        find_ball[Xi].x += d;//把x往後移一點
        f1 = C(Cj);
        find_ball[Xi].x -= d * 2;//把x往前移一點
        f0 = C(Cj);
        find_ball[Xi].x += d;//把x移回原處
        gradient[Xi].x = (f1 - f0) / 2 / d;

        find_ball[Xi].y += d;//把x往後移一點//把y往後移一點
        f1 = C(Cj);
        find_ball[Xi].y -= d * 2;//把y往前移一點
        f0 = C(Cj);
        find_ball[Xi].y += d;//把y移回原處
        gradient[Xi].y = (f1 - f0) / 2 / d;

        find_ball[Xi].z += d;//把z往後移一點
        f1 = C(Cj);
        find_ball[Xi].z -= d * 2;//把z往前移一點
        f0 = C(Cj);
        find_ball[Xi].z += d;//把z移回原處
        gradient[Xi].z = (f1 - f0) / 2 / d;
    }
    void solver()
    {//有幾個點就算幾次
        float maxMag = 0;
        float[] w = new float[N];//w=1÷有幾個點 //有幾個點就有幾個w
        for (int i = 0; i < N; i++) w[i] = 1.0f / N;//weight:加總為1
        for (int Cj = 0; Cj < (N - 1) + (N - 2); Cj++)
        {//7:N=5,(5-1)+(5-2)=>7

            for (int Xi = 0; Xi < N; Xi++)
            {
                calcGradient(Cj, Xi);//???地雷???
            }
            float gradientSum = 0;
            for (int Xi = 0; Xi < N; Xi++)
            {
                gradientSum += w[Xi] * gradient[Xi].sqrMagnitude;//公式:λ=c(x)/Σjwj|▽xjC(x)|的平方
            }
            if (gradientSum < 0.0000001) print("gradientSum too small");
            float lumda = C(Cj) / gradientSum;//注意:gradientSum值太小程式會壞//NaN: 1÷0
            for (int Xi = 0; Xi < N; Xi++)//???上方地雷???
            {       //更新
                Vector3 diff = (gradient[Xi] * -lumda * w[Xi]);//公式:△xi=-λ*wi*▽xiC(x)
                find_ball[Xi] += diff;
                if (diff.magnitude > maxMag) maxMag = diff.magnitude;
            }
        }

        //畫出新球的位置
        for (int i = 0; i < N; i++)
        {
            // Destroy(balls[i]);
            // print("delete " + balls[i]);
            GameObject throw_ball = Instantiate(sugar, find_ball[i], Quaternion.identity) as GameObject;
            throw_ball.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            balls[i] = throw_ball;
            balls[i].name = "new sphere #" + i;
            find_ball[i] = balls[i].transform.position;
        }
        //先刪除原來的絲，再畫出新的絲
        for (int i = 0; i < N - 1; i++)
        {
            Destroy(lines[i]);
            GameObject throw_line = Instantiate(sugar_line,
            new Vector3((find_ball[i].x + find_ball[i + 1].x) / 2, 1, (find_ball[i].z + find_ball[i + 1].z) / 2),
            Quaternion.FromToRotation(Vector3.up, find_ball[i] - find_ball[i + 1])) as GameObject;

            throw_line.transform.localScale = new Vector3(0.3f, (find_ball[i] - find_ball[i + 1]).magnitude / 2f, 0.3f);
            lines[i] = throw_line;
            lines[i].name = "line #" + i;
        }

        if (maxMag < 0.01f) bSolving = false;//diff如果夠大就不會再移動//距離太小，則停止計算
    }

}



