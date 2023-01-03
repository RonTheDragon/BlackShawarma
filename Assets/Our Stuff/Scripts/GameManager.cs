using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public        int         tzadokHp   = 3;
    public        int         MaxFillers = 4;
    private       int         Money;

    public Action UpdateMoney;

    public Action Loop;

    void Awake()
    {
         instance = this;
    }

    public int GetMoney()
    {
        return Money;
    }

    public void AddMoney(int m)
    {
        Money += m;
        UpdateMoney?.Invoke();
    }

    public void SetMoney(int m)
    {
        Money = m;
        UpdateMoney?.Invoke();
    }
}
