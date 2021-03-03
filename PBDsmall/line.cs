using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class line : MonoBehaviour
{
    public int N = 5;
    public GameObject[] balls;
    public GameObject[] lines;
    public Vector3[] gradient;
    public Vector3[] find_ball;
    public GameObject sugar;
    public GameObject sugar_line;
    public float[] L;
    public float[] V;
    public float[] R;
    Vector3 gravity;
    void Start()
    {
        gravity = new Vector3(0, -0.5f, 0);//重力
        balls = new GameObject[N];//指定陣列數量
        find_ball = new Vector3[N];

        //圓柱長(L)、體積(V)、半徑(R)
        R = new float[N - 1];
        V = new float[N - 1];
        L = new float[N - 1];
        for (int i = 0; i < N; i++)
        {
            GameObject throw_ball = Instantiate(sugar, new Vector3((float)i * 5f, 0, 0), Quaternion.identity) as GameObject;
            throw_ball.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            balls[i] = throw_ball;
            balls[i].name = "sphere #" + i;
            find_ball[i] = balls[i].transform.position;
        }
        lines = new GameObject[N - 1];
        for (int i = 0; i < N - 1; i++)
        {
            GameObject throw_line = Instantiate(sugar_line,
            new Vector3((balls[i].transform.position.x + balls[i + 1].transform.position.x) / 2, (balls[i].transform.position.y + balls[i + 1].transform.position.y) / 2, (balls[i].transform.position.z + balls[i + 1].transform.position.z) / 2),
            Quaternion.FromToRotation(Vector3.up, balls[i].transform.position - balls[i + 1].transform.position)) as GameObject;
            R[i] = 0.3f;//設圓柱初始半徑
            throw_line.transform.localScale = new Vector3(R[i], (balls[i].transform.position - balls[i + 1].transform.position).magnitude / 2f, R[i]);
            lines[i] = throw_line;
            lines[i].name = "line #" + i;

            //圓柱長 = sqrt (x平方 + y平方 + z平方) 
            L[i] = Mathf.Sqrt((find_ball[i].x - find_ball[i + 1].x) * (find_ball[i].x - find_ball[i + 1].x)
            + (find_ball[i].y - find_ball[i + 1].y) * (find_ball[i].y - find_ball[i + 1].y)
            + (find_ball[i].z - find_ball[i + 1].z) * (find_ball[i].z - find_ball[i + 1].z));

            //圓柱體積 = π × (r平方) × 高
            V[i] = Mathf.PI * (0.3f * 0.3f) * L[i];
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            print("pressed button 5 ");
            balls[4].AddComponent<move_ball>();
        }
        drawNewline();
        for (int i = 1; i < N - 1; i++)
        {
            Vector3 diff1 = balls[i].transform.position - balls[i - 1].transform.position;//v向量(x,y,z),mag向量轉長度
            Vector3 diff2 = balls[i].transform.position - balls[i + 1].transform.position;
            Vector3 diff3 = -(diff1 + diff2) / 2;
            balls[i].transform.position += diff3 + gravity;
        }
        for (int i = 0; i < N; i++)
        {
            find_ball[i] = balls[i].transform.position;
        }

    }

    void drawNewline()
    {

        for (int i = 0; i < N - 1; i++)
        {
            Destroy(lines[i]);
            GameObject throw_line = Instantiate(sugar_line,
            new Vector3((balls[i].transform.position.x + balls[i + 1].transform.position.x) / 2, (balls[i].transform.position.y + balls[i + 1].transform.position.y) / 2, (balls[i].transform.position.z + balls[i + 1].transform.position.z) / 2),
            Quaternion.FromToRotation(Vector3.up, balls[i].transform.position - balls[i + 1].transform.position)) as GameObject;

            //算出新的半徑再畫
            throw_line.transform.localScale = new Vector3(R[i], (balls[i].transform.position - balls[i + 1].transform.position).magnitude / 2f, R[i]);
            lines[i] = throw_line;
            lines[i].name = "newline #" + i;
            //圓柱長 = sqrt (x平方 + y平方 + z平方) 
            L[i] = Mathf.Sqrt((find_ball[i].x - find_ball[i + 1].x) * (find_ball[i].x - find_ball[i + 1].x)
                + (find_ball[i].y - find_ball[i + 1].y) * (find_ball[i].y - find_ball[i + 1].y)
                + (find_ball[i].z - find_ball[i + 1].z) * (find_ball[i].z - find_ball[i + 1].z));

            R[i] = Mathf.Sqrt(V[i] / Mathf.PI / L[i]);

        }
    }
}

