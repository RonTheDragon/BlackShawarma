using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Player;

    public int MaxTzadokHp = 3;
    public int MaxFillers  = 4;
    public int TzadokMaxHP = 4;
    public int ManInLine = 3;
    private int _money;
    private float _moneyMultiplier=1;
    private int _tzadokHp;

    public Action UpdateMoney;
    public Action UpdateTazdokHp;
    public Action TakeDamage;

    public Action Loop;
    public Action OnVictoryScreen;
    public Action OnLoseScreen;
    public Action OnEndLevel;
    public EnemySpawner EnemySpawner;
    public LevelTimer LvlTimer => GetComponent<LevelTimer>();
    public ComboManager CM => GetComponent<ComboManager>();

    public LayerMask NotPlayerLayer;

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
        _money += (int)(m* _moneyMultiplier);
        Debug.Log($"Reward: {m} * {_moneyMultiplier} = {(int)(m * _moneyMultiplier)}");
        UpdateMoney?.Invoke();
    }

    public void SetMoney(int m)
    {
        _money = m;
        UpdateMoney?.Invoke();
    }

    public void SetMoneyMultiplier(float m)
    {
        _moneyMultiplier = m;
    }

    #endregion
    #region UpdateTazdokUiHp
    public int GetTazdokHp()
    {
        return _tzadokHp;
    }

    public void SetTazdokHp(int m)
    {
        _tzadokHp = m;
        UpdateTazdokHp?.Invoke();
    }

    public void AddTazdokHp(int m)
    {
        _tzadokHp += m;
        UpdateTazdokHp?.Invoke();
    }

    public void TazdokTakeDamage(int m)
    {
        Debug.Log("pain");
        _tzadokHp -= m;
        TakeDamage?.Invoke();
        UpdateTazdokHp?.Invoke();
        CM.ResetCombo();

        if (_tzadokHp <= 0)
        {
            OnLoseScreen?.Invoke();
            OnEndLevel?.Invoke();
            EnemySpawner.ClearingLevel();
        }
    }

    #endregion
    public void DidWeWin()
    {
        if (EnemySpawner.HowManyEnemiesInTheStore() == 0 && LvlTimer.IsDone == true && _tzadokHp > 0)
        {
            OnVictoryScreen?.Invoke();
            OnEndLevel?.Invoke();
        }
    }
}
