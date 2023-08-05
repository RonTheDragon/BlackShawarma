using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Player;

    public int MaxTzadokHp = 3;
    public int MaxFillers  = 4;
    public int TzadokMaxHP = 4;
    private int _money;
    private float _moneyMultiplier=1;
    private int tzdakaBonus = 0;
    [HideInInspector] public int tzdakaLvl = 0;
    [HideInInspector] public bool UsedChili = false;

    public float EnemiesBehindCalmerBy = 1.5f;
    public float CalmEnemiesStayCalmBy = 1.5f;
    public float FoodCalmingEffect = 20;
    private int _tzadokHp;
    [HideInInspector] public int HappyCustomers = 0;

    public Action UpdateMoney;
    public Action UpdateTazdokHp;
    public Action TakeDamage;

    public Action Loop;
    public Action OnVictoryScreen;
    public Action OnLoseScreen;
    public Action OnEndLevel;
    public Action OnStartLevel;
    public Action OnAmmoUpdate;

    public Action OnPickUpSack;
    public Action OnPlaceDownSack;

    public Action<bool> OnTryToBuy;

    [HideInInspector] public SideOrderUI UsedOrder;
    [HideInInspector] public Action<SideOrderUI> OnOrderMaximize;
    public EnemySpawner EnemySpawner;
    public LevelTimer LvlTimer => GetComponent<LevelTimer>();
    public ComboManager CM => GetComponent<ComboManager>();
    

    public LayerMask NotPlayerLayer;

    [HideInInspector] public Shop TheShop;

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
        TzdakaUpdate();
        _money += (int)(m * _moneyMultiplier + tzdakaBonus);
        Debug.Log($"Reward: {m} * {_moneyMultiplier} + {tzdakaBonus} = {(int)(m * _moneyMultiplier+ tzdakaBonus)}");
        UpdateMoney?.Invoke();
    }

    public void RemoveMoney(int m)
    {
        _money -= m;
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
        AudioManager.instance.PlayOneShot(FMODEvents.instance.hit, transform.position);
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
        if (EnemySpawner.HowManyEnemiesInTheStore() == 0 && LvlTimer.GetIsTimeDone() == true && _tzadokHp > 0)
        {
            OnVictoryScreen?.Invoke();
            OnEndLevel?.Invoke();
        }
    }

    public void MaximizeOrder(SideOrderUI ui)
    {
        if (UsedOrder == ui)
        {
            OnOrderMaximize?.Invoke(null);
            UsedOrder = null;
            return;
        }
        UsedOrder = ui;
        OnOrderMaximize?.Invoke(ui);
    }

    public void UnMaximizeOrder(SideOrderUI ui)
    {
        if (ui == null) return;
        if (UsedOrder == ui)
        {
            OnOrderMaximize?.Invoke(null);
            UsedOrder = null;
        }
    }

    public void TzdakaUpdate()
    {
        if (tzdakaLvl == 0)
        {
            tzdakaBonus = 0;
        }
        else if (tzdakaLvl == 1)
        {
            tzdakaBonus = Random.Range(1, 3);
        }
        else if (tzdakaLvl == 2)
        {
            tzdakaBonus = Random.Range(7, 13);
        }
        else if (tzdakaLvl == 3)
        {
            tzdakaBonus = Random.Range(15, 20);
        }
    }
}
