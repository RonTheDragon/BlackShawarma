using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Random = System.Random;
using System.Runtime.InteropServices;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float CurrentRage, MaxRage, CalmEnoughToEat;
    [SerializeField] float AngerSmokeAmount = 1;
    [SerializeField] ParticleSystem AngerSmoke;
    public int PlaceInLane;
    public int WhichLane;
    [SerializeField] bool _falefelEater;
    [SerializeField] bool _eggplantEater;
    [SerializeField] bool _friesEater;
    [SerializeField] GameObject Canvas;
    [SerializeField] TMP_Text OrderText;
    [SerializeField] float mintime=60 ;
    [SerializeField] float maxtime = 180;
    [SerializeField] Vector2 randompayment = new Vector2(10,25);

    NavMeshAgent agent => GetComponent<NavMeshAgent>();
    EnemySpawner Spawner;
    Action OnRage;
    Vector3 destination;
     int _enemyMaxPayment;
     int _enemyMinPayment;
    public int CurrentPayment;
    float time;
   


    Camera PlayerCamera => Camera.main;

    GameManager GM => GameManager.instance;

    Action loop;

    List<BuildOrder.Fillers> Order = new List<BuildOrder.Fillers>();

    bool done;


    public void Spawn(EnemySpawner spawner)
    {
        time = 0;
        Spawner = spawner;
        done = false;
        SetEnemyPayment();
        GenerateRandomOrder();
    }


    void Start()
    {
        OnRage += () => { done = true; CurrentRage = 0; AngerSmoke.Emit(100); Spawner.RemoveOnLane(WhichLane, PlaceInLane); GM.tzadokHp--; };
        loop += Movement;
        loop += Rage;
        loop += ShowOrder;
        loop += EnemyTimer;
    }


    void Update()
    {
        loop?.Invoke();
    }


    void Movement()
    {
        if (done)
        {
            agent.SetDestination(Spawner.DoorSpawnPoint.position);
            if (Vector3.Distance(transform.position, Spawner.DoorSpawnPoint.position) < 2)
            {
                Spawner.RemoveEnemy(this);
                gameObject.SetActive(false);
            }
        }
        else
        {
            agent.SetDestination(destination);
        }
    }
    void Rage()
    {
        ParticleSystem.EmissionModule emission = AngerSmoke.emission;
        emission.rateOverTime = CurrentRage * 0.1f * AngerSmokeAmount;
        if (CurrentRage < 0)
        {
            CurrentRage = 0;
        }
        else if (CurrentRage < MaxRage)
        {
            if (!done)
            {
                CurrentRage += Time.deltaTime;
            }
        }
        else
        {
            OnRage?.Invoke();
            GM.tzadokHp--;
        }
    }

    void ShowOrder() 
    {
        if (CalmEnoughToEat >= CurrentRage && !done) //if calm enough to eat
        {
            if (Canvas.activeSelf==false)
            Canvas.gameObject.SetActive(true);
            Canvas.transform.LookAt(PlayerCamera.transform.position);
        }
        else
        {
            if (Canvas.activeSelf == true)
                Canvas.gameObject.SetActive(false);
        }
    }

    public void Eat(Edible.Food f)
    {
        if (CaniEAT(f))
        {
            CurrentRage -= 10;
        }
        else
        {

            CurrentRage += 10;
        }
    }

    public void EatPita(List<BuildOrder.Fillers> pita)
    {
        bool CorrectOrder = true;

        if (pita.Count == Order.Count && CalmEnoughToEat>=CurrentRage  ) // if the pita and the order contains the same amount
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
            CurrentRage += 10;
        }
    }

    void HappyCustomer()
    {
        done = true;
        CurrentRage = 0;
        GM.AddMoney(CurrentPayment);
        GM.UpdateMoney?.Invoke();
    }

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
    }
    bool CaniEAT(Edible.Food food)
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
        int count = Enum.GetValues(typeof(BuildOrder.Fillers)).Length;

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
        OrderText.text = orderInText;
    }
    void SetEnemyPayment()
    {
        var random = new Random();
        _enemyMaxPayment = random.Next((int)randompayment.x,(int)randompayment.y);
        _enemyMinPayment = _enemyMaxPayment / 2;
        CurrentPayment = _enemyMaxPayment;
    }
    void EnemyTimer()
    {
        time += Time.deltaTime;
        if (time>mintime&&time<maxtime)
        {         
            float n = (1 - (time - mintime) / (maxtime - mintime));//min and max of the time for the payment 
            CurrentPayment = (int)Mathf.Lerp(_enemyMinPayment,_enemyMaxPayment, n);

        }
    }
}
