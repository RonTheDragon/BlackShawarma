using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Random = System.Random;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float          _currentRage, _maxRage, _calmEnoughToEat , _startingRage;
    [SerializeField] float          _angerSmokeAmount = 1;
    [SerializeField] ParticleSystem _angerSmoke;
    [SerializeField] private ParticleSystem _veryAngrySmoke;
    [SerializeField] private ParticleSystem _happy;
    [SerializeField] private ParticleSystem _veryHappy;
    public           int            PlaceInLane,   WhichLane;

    public           Sprite          HappyPicture;
    public           Sprite          AngryPicture;
    public           Sprite          SideOrderPanel;
    public           Sprite          RequestedFood;
    public           int             CustomerNumber;

    [SerializeField] bool           _falefelEater, _eggplantEater, _friesEater;
    [SerializeField] GameObject     _canvas;
    [SerializeField] private List<GameObject> _orderFillers;
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
    public int EnemyDamage = 1;


    [SerializeField] private Animator _anim;

    private Camera PlayerCamera => Camera.main;

    private GameManager _gm => GameManager.Instance;

    private Action Loop;

    public Action<float,bool> OnRageAmountChange;
    public Action OnBeingShot;

    [HideInInspector] public List<BuildOrder.Fillers> Order = new List<BuildOrder.Fillers>();

    private bool _done;

    private Vector3 PreviousPos;


    public void Spawn(EnemySpawner spawner,int num)
    {
        _currentRage   = _startingRage;
        _time          = 0;
        _spawner       = spawner;
        _done          = false;
        _agent.enabled = true;
        CustomerNumber= num;
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
        if (!_done)
        {
            _agent.SetDestination(_destination);
        }
        else
        {
            _agent.SetDestination(_spawner.DoorSpawnPoint.position);
            if (Vector3.Distance(transform.position, _spawner.DoorSpawnPoint.position) < 2)
            {
                LeavingTheStore();
            }
        }
        if (PreviousPos != transform.position)
        {
            _anim.SetBool("Walk", true);
        }
        else
        {
            _anim.SetBool("Walk", false);
        }
        PreviousPos = transform.position;
    }


    private void Rage()
    {
        ParticleSystem.EmissionModule emission = _angerSmoke.emission;
        if (_done) 
        {
            emission.rateOverTime = 0;
            return; 
        }
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
    {if (_done) return;
        if (CanIEat(f))
        {
            _currentRage -= 10;
            _happy.Emit(1);
        }
        else
        {
            _currentRage += 10;
            _angerSmoke.Emit(1);
        }
        OnBeingShot?.Invoke();
    }

    public void EatPita(List<BuildOrder.Fillers> pita)
    {
        if (_done) return;
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
            _veryHappy.Emit(1);
        }
        else
        {
            _currentRage += 10;
            _angerSmoke.Emit(1);
            OnBeingShot?.Invoke();
        }
    }

    #region CustomerReaction
    private void HappyCustomer()
    {
        _gm.AddMoney(CurrentPayment);

        RemoveCustomer();
    }

    private void MadCustomer()
    {
        _veryAngrySmoke.Emit(1);
        _gm.TazdokTakeDamage(1);

        RemoveCustomer();
    }

    private void RemoveCustomer()
    {
        _done = true;
        _currentRage = 0;
        _spawner.RemoveOnLane(WhichLane, PlaceInLane);

        OnRageAmountChange = null;
        if (SideOrder!=null)
        Destroy(SideOrder.gameObject);
    }
    #endregion

    private void LeavingTheStore()
    {
        _spawner.RemoveEnemy(this);
        _gm.DidWeWin();
        gameObject.SetActive(false);
    }

    public void InstantlyRemoveCustomer()
    {
        StartCoroutine("DeletingEnemy");
    }

    private System.Collections.IEnumerator DeletingEnemy()
    {
        yield return null;

        if (!_done)
        {
            RemoveCustomer();
        }
        LeavingTheStore();
    }

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

        int FillerAmount = UnityEngine.Random.Range(1, _gm.MaxFillers+1);
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

        //string orderInText = "Order:";
        //foreach(BuildOrder.Fillers f in Order)
        //{
        //    orderInText += $"\n{f}";
        //}
        //_orderText.text = orderInText;

        foreach(GameObject go in _orderFillers)
        {
            go.SetActive(false);
        }

        foreach (BuildOrder.Fillers filler in Order)
        {
            switch (filler)
            {
                case BuildOrder.Fillers.Humus:
                    _orderFillers[0].SetActive(true);
                    break;
                case BuildOrder.Fillers.Pickles:
                    _orderFillers[1].SetActive(true);
                    break;
                case BuildOrder.Fillers.Cabbage:
                    _orderFillers[2].SetActive(true);
                    break;
                case BuildOrder.Fillers.Onions:
                    _orderFillers[3].SetActive(true);
                    break;
                case BuildOrder.Fillers.Salad:
                    _orderFillers[4].SetActive(true);
                    break;
                case BuildOrder.Fillers.Spicy:
                    _orderFillers[5].SetActive(true);
                    break;
                case BuildOrder.Fillers.Amba:
                    _orderFillers[6].SetActive(true);
                    break;
                case BuildOrder.Fillers.Thina:
                    _orderFillers[7].SetActive(true);
                    break;
            }
        }
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
