using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smallone : MonoBehaviour
{
    // Start is called before the first frame update
    public int N = 3;
    float d0 = 0.8f, angle0 = Mathf.Sin(Mathf.PI);
    public GameObject[] balls;
    public GameObject[] lines;
    public Vector3[] gradient;
    public Vector3[] find_ball;
    public GameObject sugar;
    public GameObject sugar_line;
    //bool bSolving = false;
    Rigidbody rb_ball;
    void Start()
    {
        balls = new GameObject[N];//指定陣列數量
        gradient = new Vector3[N];
        find_ball = new Vector3[N];//紀錄球的位置
        for (int i = 0; i < N; i++)
        {
            // Vector3 p1 = balls[i].transform.position;
            // Vector3 p2 = balls[i + 1].transform.position;
            // lines[i].transform.position = (p1 + p2) / 2;
            // lines[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, p1 - p2);
            // lines[i].transform.localScale = new Vector3(0.3f, (p1 - p2).magnitude / 2f, 0.3f);
            GameObject throw_ball = Instantiate(sugar, new Vector3((float)i, 1, Random.Range(1f, 15f)), Quaternion.identity) as GameObject;
            throw_ball.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            balls[i] = throw_ball;
            balls[i].name = "sphere #" + i;
            gradient[i] = new Vector3();
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


    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < N; i++)
            {
                Destroy(balls[i]);
                print("delete " + balls[i]);
            }
            solver();

        }

    }
    float C1(Vector3[] balls)
    {//公式:|x1-x2|平方-d平方
        Vector3 v1 = balls[1] - balls[0];
        return v1.sqrMagnitude - d0 * d0;
    }
    float C2(Vector3[] balls)
    {
        Vector3 v2 = balls[2] - balls[1];
        return v2.sqrMagnitude - d0 * d0;
    }
    float Cbend(Vector3[] balls)
    {
        Vector3 v1 = balls[1] - balls[0];
        Vector3 v2 = balls[2] - balls[1];
        return Vector3.Angle(v1, v2) - angle0;//算出兩向量的夾角
    }
    float C(int Cj)
    {
        if (Cj == 0) return C1(find_ball);
        if (Cj == 1) return C2(find_ball);
        if (Cj == 2) return Cbend(find_ball);
        return 0;
    }
    void calcGradient(int Cj, int Xi)
    {//input(第幾個constrain,第幾個點)
        float d = 0.0001f;
        float f0, f1;
        find_ball[Xi].x += d;//把x往後移一點
        print("find_ball.x1:" + find_ball[Xi].x);
        f1 = C(Cj);
        print("f1.x :" + f1);
        find_ball[Xi].x -= d * 2;//把x往前移一點
        print("find_ball.x2:" + find_ball[Xi].x);
        f0 = C(Cj);
        print("f0.x :" + f0);
        find_ball[Xi].x += d;//把x移回原處
        print("find_ball.x3:" + find_ball[Xi].x);
        gradient[Xi].x = (f1 - f0) / 2 / d;
        print("Xi.x :" + gradient[Xi]);

        find_ball[Xi].y += d;//把y往後移一點
        f1 = C(Cj);
        print("f1.y :" + f1);
        find_ball[Xi].y -= d * 2;//把y往前移一點
        f0 = C(Cj);
        print("f0.y :" + f0);
        find_ball[Xi].y += d;//把y移回原處
        gradient[Xi].y = (f1 - f0) / 2 / d;
        print("Xi.y :" + gradient[Xi]);

        find_ball[Xi].z += d;//把z往後移一點
        f1 = C(Cj);
        print("f1.z :" + f1);
        find_ball[Xi].z -= d * 2;//把z往前移一點
        f0 = C(Cj);
        print("f0.z :" + f0);
        find_ball[Xi].z += d;//把z移回原處
        gradient[Xi].z = (f1 - f0) / 2 / d;
        print("Xi.z :" + gradient[Xi]);
    }
    void solver()
    {//有幾個點就算幾次
        for (int Cj = 0; Cj < 3; Cj++)
        {
            float[] w = { 0.33333f, 0.33333f, 0.33333f };//w=1÷有幾個點
            for (int Xi = 0; Xi < 3; Xi++)
            {
                calcGradient(Cj, Xi);
            }
            float gradientSum = 0;
            for (int Xi = 0; Xi < 3; Xi++)
            {
                gradientSum += w[Xi] * gradient[Xi].sqrMagnitude;
                print("gradientSum :" + gradientSum);
            }
            float lumda = C(Cj) / gradientSum;//公式:λ=c(x)/Σjwj|▽xjC(x)|的平方
            for (int Xi = 0; Xi < 3; Xi++)
            {       //更新
                find_ball[Xi] += gradient[Xi] * -lumda * w[Xi];//公式:△xi=-λ*wi*▽xiC(x)
                print("find_ball" + Xi + " :" + find_ball[Xi]);
            }
        }
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


    }

}
