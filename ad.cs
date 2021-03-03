﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class dfloat
{//需要每個都加上public, 不能用 public:
    static int N = 6; //要加 static, 後面才能用
    float[] v = new float[N + 1]; // change this to your favorite "pretend real."
                                  // and all references to floats. Use a typedef or a define or whatever.
    public dfloat()
    { //初始化, 都設成0
        for (int i = 0; i <= N; i++) v[i] = 0.0f;
    }
    public dfloat(float s)
    { //初始第1個原值, 其他(微分值)都設0
        v[0] = s;
        for (int i = 1; i <= N; i++) v[i] = 0.0f;
    }
    //參考(c++用&表示，c#用ref表示)
    public ref float val()
    { //reference 參考
        return ref v[0]; //這裡有改, 因為要一致
    }
    public ref float val(int i)
    { //reference 參考
        return ref v[i]; //這裡有改, 因為要一致
    }
    public void val(float s)
    { //上面是整數index(取出),下面是float值(設定)
        v[0] = s;
    }
    public void val(int i, float s)
    { //設定值
        v[i] = s;
    }

    //https://stackoverflow.com/questions/599367/why-can-not-be-overloaded-in-c
    //"operator = is applied to references 原因是因為 = 用在 reference, 就簡單多了!
    //Note also that, unlike C++, the = assignment operator cannot be overloaded in C#.
    //if you overload a binary operator, such as +, += is also overloaded. 
    //所以我把整套 =, +=, -=, *=, /= 都刪掉了! 
    public static dfloat operator -(dfloat a)
    { //負號
        dfloat c = new dfloat();
        for (int i = 0; i <= N; i++) c.v[i] = -a.v[i];
        return c;
    }

    //friend(C++) 改成 public static(C#)
    public static dfloat operator +(dfloat a, dfloat b)
    {
        dfloat c = new dfloat();
        for (int i = 0; i <= N; i++) c.v[i] = a.v[i] + b.v[i];
        return c;
    }
    public static dfloat operator -(dfloat a, dfloat b)
    {
        dfloat c = new dfloat();
        for (int i = 0; i <= N; i++) c.v[i] = a.v[i] - b.v[i];
        return c;
    }
    public static dfloat operator *(dfloat a, dfloat b)
    {
        dfloat c = new dfloat();
        c.v[0] = a.v[0] * b.v[0];
        for (int i = 1; i <= N; i++) c.v[i] = a.v[i] * b.v[0] + a.v[0] * b.v[i];
        return c;
    }
    public static dfloat operator /(dfloat a, dfloat b)
    {
        dfloat c = new dfloat();
        c.v[0] = a.v[0] / b.v[0];
        float g = b.v[0] * b.v[0];
        for (int i = 1; i <= N; i++) c.v[i] = (a.v[i] * b.v[0] - a.v[0] * b.v[i]) / g;
        return c;
    }
    public static dfloat operator +(float s, dfloat a)
    {
        dfloat c = new dfloat();
        c.v[0] = s + a.v[0];
        for (int i = 1; i <= N; i++) c.v[i] = a.v[i];
        return c;
    }
    public static dfloat operator +(dfloat a, float s)
    {
        dfloat c = new dfloat();
        c.v[0] = a.v[0] + s;
        for (int i = 1; i <= N; i++) c.v[i] = a.v[i];
        return c;
    }
    public static dfloat operator -(float s, dfloat a)
    {
        dfloat c = new dfloat();
        c.v[0] = s - a.v[0];
        for (int i = 1; i <= N; i++) c.v[i] = -a.v[i];
        return c;
    }
    public static dfloat operator -(dfloat a, float s)
    {
        dfloat c = new dfloat();
        c.v[0] = a.v[0] - s;
        for (int i = 1; i <= N; i++) c.v[i] = a.v[i];
        return c;
    }
    public static dfloat operator *(float s, dfloat a)
    {
        dfloat c = new dfloat();
        for (int i = 0; i <= N; i++) c.v[i] = s * a.v[i];
        return c;
    }
    public static dfloat operator *(dfloat a, float s)
    {
        dfloat c = new dfloat();
        for (int i = 0; i <= N; i++) c.v[i] = a.v[i] * s;
        return c;
    }
    public static dfloat operator /(float s, dfloat a)
    {
        dfloat c = new dfloat();
        c.v[0] = s / a.v[0];
        float g = a.v[0] * a.v[0];
        for (int i = 1; i <= N; i++) c.v[i] = -s * a.v[i] / g;
        return c;
    }
    public static dfloat operator /(dfloat a, float s)
    {
        dfloat c = new dfloat();
        for (int i = 0; i <= N; i++) c.v[i] = a.v[i] / s;
        return c;
    }
    public static dfloat dsqrt(dfloat a)
    {
        dfloat c = new dfloat();
        c.v[0] = Mathf.Sqrt(a.v[0]);
        for (int i = 1; i <= N; i++) c.v[i] = 0.5f * a.v[i] / c.v[0];
        return c;
    }
    public static dfloat dacos(dfloat a)
    {
        dfloat c = new dfloat();
        c.v[0] = (float)Mathf.Acos(a.v[0]);
        float g = -1.0f / Mathf.Sqrt(1 - a.v[0] * a.v[0]);
        for (int i = 1; i <= N; i++) c.v[i] = a.v[i] * g;
        return c;
    }
}
