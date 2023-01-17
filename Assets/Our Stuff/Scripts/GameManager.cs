using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public        int         TzadokHp    = 3;
    public        int         MaxTzadokHp = 3;
    public        int         MaxFillers  = 4;
    private       int         Money;

    public Action UpdateMoney;

    public Action       Loop;
    public Action       OnVictoryScreen;
    public EnemySpawner EnemySpawner;
    public LevelTimer LvlTimer => GetComponent<LevelTimer>();

    void Awake()
    {
         Instance = this;
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
  
    public void DidWeWin()
    {
        if (EnemySpawner.HowManyEnemiesInTheStore() == 0 && LvlTimer.IsDone == true && TzadokHp > 0)
        {
            OnVictoryScreen?.Invoke();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
    }
}
