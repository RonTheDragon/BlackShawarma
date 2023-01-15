using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Random = System.Random;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float          _currentRage, _maxRage, _calmEnoughToEat;
    [SerializeField] float          _angerSmokeAmount = 1;
    [SerializeField] ParticleSystem _angerSmoke;
    public           int            PlaceInLane,   WhichLane;
    public           Sprite          Picture;
    [SerializeField] bool           _falefelEater, _eggplantEater, _friesEater;
    [SerializeField] GameObject     _canvas;
    [SerializeField] TMP_Text       _orderText;
    [SerializeField] float          _minTime       = 60;
    [SerializeField] float          _maxTime       = 180;
    [SerializeField] Vector2        _randompayment = new Vector2(10,25);

    NavMeshAgent _agent => GetComponent<NavMeshAgent>();
    EnemySpawner _spawner;
    Action       _onRage;
    Vector3      _destination;
    int          _enemyMaxPayment, _enemyMinPayment;
    float        _time;
    public int   CurrentPayment;
    public SideOrderUI SideOrder;
   


    Camera PlayerCamera => Camera.main;

    GameManager GM => GameManager.instance;

    Action Loop;

    public Action<float,bool> OnRageAmountChange;

    [HideInInspector] public List<BuildOrder.Fillers> Order = new List<BuildOrder.Fillers>();

    bool _done;


    public void Spawn(EnemySpawner spawner)
    {
        _time          = 0;
        _spawner       = spawner;
        _done          = false;
        _agent.enabled = true;
        SetEnemyPayment();
        GenerateRandomOrder();
    }


    private void Start()
    {
        _onRage += MadCustomer;
        Loop += Movement;
        Loop += Rage;
        Loop += ShowOrder;
        Loop += EnemyTimer;
    }


    private void Update()
    {
        Loop?.Invoke();
    }


    private void Movement()
    {
        if (_done)
        {
            _agent.SetDestination(_spawner.DoorSpawnPoint.position);
            if (Vector3.Distance(transform.position, _spawner.DoorSpawnPoint.position) < 2)
            {
                _spawner.RemoveEnemy(this);
                gameObject.SetActive(false);
            }
        }
        else
        {
            _agent.SetDestination(_destination);
        }
    }
    private void Rage()
    {
        ParticleSystem.EmissionModule emission = _angerSmoke.emission;
        emission.rateOverTime = _currentRage * 0.1f * _angerSmokeAmount;
        if (_currentRage < 0)
        {
            _currentRage = 0;
        }
        else if (_currentRage < _maxRage)
        {
            if (!_done)
            {
                _currentRage += Time.deltaTime;
            }
        }
        else
        {
            _onRage?.Invoke();
            GM.tzadokHp--;
        }
        OnRageAmountChange?.Invoke(1 -(_currentRage / _maxRage), _currentRage > _calmEnoughToEat);
    }

    private void ShowOrder() 
    {
        if (_calmEnoughToEat >= _currentRage && !_done) //if calm enough to eat
        {
            if (_canvas.activeSelf == false)
            _canvas.gameObject.SetActive(true);
            _canvas.transform.LookAt(PlayerCamera.transform.position);
        }
        else
        {
            if (_canvas.activeSelf == true)
                _canvas.gameObject.SetActive(false);
        }
    }

    public void Eat(Edible.Food f)
    {
        if (CanIEat(f))
        {
            _currentRage -= 10;
        }
        else
        {
            _currentRage += 10;
        }
    }

    public void EatPita(List<BuildOrder.Fillers> pita)
    {
        bool CorrectOrder = true;

        if (pita.Count == Order.Count && _calmEnoughToEat >= _currentRage  ) // if the pita and the order contains the same amount
        {
            for (int i = 0; i < Order.Count; i++) //going over the order
            {
                if (!pita.Contains(Order[i])) // if the pita doesnt contains what the order requires
                {
                    CorrectOrder = false; break; // then the order is incorrent
                }
            }
        }
        else
        {
            CorrectOrder = false;
        }

        if (CorrectOrder)
        {
            HappyCustomer();           
        }
        else
        {
            _currentRage += 10;
        }
    }

    #region CustomerReaction
    private void HappyCustomer()
    {
        RemoveSideOrder();
        _done        = true;
        _currentRage = 0;
        GM.AddMoney(CurrentPayment);
        _spawner.RemoveOnLane(WhichLane, PlaceInLane);
    }

    private void MadCustomer()
    {
        RemoveSideOrder();
        _done        = true;
        _currentRage = 0;
        _angerSmoke.Emit(100);
        _spawner.RemoveOnLane(WhichLane, PlaceInLane);
        GM.tzadokHp--;
    }

    private void RemoveSideOrder()
    {
        OnRageAmountChange = null;
        Destroy(SideOrder.gameObject);
    }
    #endregion

    public void SetDestination(Vector3 pos)
    {
        _destination = pos;
    }

    bool CanIEat(Edible.Food food)
    {
        bool _didIEatit = false;
        switch (food)
        {
            case Edible.Food.Falafel:
                _didIEatit = _falefelEater;
                break;
            case Edible.Food.Eggplant:
                _didIEatit = _eggplantEater;
                break;
            case Edible.Food.Fries:
                _didIEatit = _friesEater;
                break;
            default:
                break;
        }
        return _didIEatit;
    }

    public void GenerateRandomOrder()
    {
        List<BuildOrder.Fillers> RandomOrder = new List<BuildOrder.Fillers>();

        int FillerAmount = UnityEngine.Random.Range(1, GM.MaxFillers+1);
        int count        = Enum.GetValues(typeof(BuildOrder.Fillers)).Length;

        for (int i = 0; i < FillerAmount; i++)
        {
            BuildOrder.Fillers fill = (BuildOrder.Fillers)UnityEngine.Random.Range(0, count);
            if (!RandomOrder.Contains(fill))
            {
                RandomOrder.Add(fill);
            }
            else
            {
                i--;
            }
        }

        Order = RandomOrder;

        string orderInText = "Order:";
        foreach(BuildOrder.Fillers f in Order)
        {
            orderInText += $"\n{f}";
        }
        _orderText.text = orderInText;
    }

    void SetEnemyPayment()
    {
        Random random    = new Random();
        _enemyMaxPayment = random.Next((int)_randompayment.x,(int)_randompayment.y);
        _enemyMinPayment = _enemyMaxPayment / 2;
        CurrentPayment   = _enemyMaxPayment;
    }

    void EnemyTimer()
    {
        _time += Time.deltaTime;
        if (_time > _minTime && _time < _maxTime)
        {         
            float n = (1 - (_time - _minTime) / (_maxTime - _minTime));//min and max of the time for the payment 
            CurrentPayment = (int)Mathf.Lerp(_enemyMinPayment,_enemyMaxPayment, n);

        }
    }
}
