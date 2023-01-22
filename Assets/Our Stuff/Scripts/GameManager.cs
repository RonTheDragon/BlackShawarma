using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public        int         MaxTzadokHp = 3;
    public        int         MaxFillers  = 4;
    public        int         TzadokMaxHP = 4;
    private       int         _money;
    private       int         _tzadokHp;

    public Action UpdateMoney;
    public Action UpdateTazdokHp;

    public Action       Loop;
    public Action       OnVictoryScreen;
    public EnemySpawner EnemySpawner;
    public LevelTimer LvlTimer => GetComponent<LevelTimer>();

    void Awake()
    {
         Instance = this;
    }
    #region Money
    public int GetMoney()
    {
        return _money;
    }

    public void AddMoney(int m)
    {
        _money += m;
        UpdateMoney?.Invoke();
    }

    public void SetMoney(int m)
    {
        _money = m;
        UpdateMoney?.Invoke();
    }
    #endregion
    #region UpdateTazdokUiHp
    public int GetTazdokHp()
    {
        return _tzadokHp;
    }

    public void AddTazdokHp(int m)
    {
        _tzadokHp += m;
        UpdateTazdokHp?.Invoke();
    }

    public void TazdokGetDamage(int m)
    {
        _tzadokHp -= m;
        UpdateTazdokHp?.Invoke();
    }
    #endregion
    public void DidWeWin()
    {
        if (EnemySpawner.HowManyEnemiesInTheStore() == 0 && LvlTimer.IsDone == true && _tzadokHp > 0)
        {
            OnVictoryScreen?.Invoke();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
    } 
}
