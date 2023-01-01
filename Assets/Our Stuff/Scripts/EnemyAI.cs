using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Random = System.Random;

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

    NavMeshAgent agent => GetComponent<NavMeshAgent>();
    EnemySpawner Spawner;
    Action OnRage;
    Vector3 destination;
     public int enemyworth { get; set; }

    Camera PlayerCamera => Camera.main;


    Action loop;

    List<BuildOrder.Fillers> Order = new List<BuildOrder.Fillers>();

    bool done;


    public void Spawn(EnemySpawner spawner)
    {
        Spawner = spawner;
        done = false;
        Price();
        GenerateRandomOrder();
    }


    void Start()
    {
        OnRage += () => { done = true; CurrentRage = 0; AngerSmoke.Emit(100); Spawner.RemoveOnLane(WhichLane, PlaceInLane); GameManager.instance.tazdokHp--; };
        loop += Movement;
        loop += Rage;
        loop += ShowOrder;
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
            GameManager.instance.tazdokHp--;
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

    public void Eat(Adible.Food f)
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
        GameManager.instance.Money += enemyworth;
    }

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
    }
    bool CaniEAT(Adible.Food food)
    {
        bool _didIEatit = false;
        switch (food)
        {
            case Adible.Food.Falafel:
                _didIEatit = _falefelEater;
                break;
            case Adible.Food.Eggplant:
                _didIEatit = _eggplantEater;
                break;
            case Adible.Food.Fries:
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

        int FillerAmount = UnityEngine.Random.Range(1, GameManager.instance.MaxFillers+1);
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
    void Price()
    {
        var random = new Random();
        enemyworth = random.Next(10, 25);
    }
}
