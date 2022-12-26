using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float CurrentRage, MaxRage;
    [SerializeField] float AngerSmokeAmount = 1;
    [SerializeField] ParticleSystem AngerSmoke;
    public int PlaceInLane;
    public int WhichLane;
    [SerializeField] bool _falefelEater;
    [SerializeField] bool _eggplantEater;
    [SerializeField] bool _friesEater;


    NavMeshAgent agent => GetComponent<NavMeshAgent>();
    EnemySpawner Spawner;
    Action OnRage;
    Vector3 destination;


    Action loop;

    List<BuildOrder.Fillers> Order = new List<BuildOrder.Fillers>();

    bool done;


    public void Spawn(EnemySpawner spawner)
    {
        Spawner = spawner;
        done = false;
        GenerateRandomOrder();
    }


    void Start()
    {
        OnRage += () => { done = true; CurrentRage = 0; AngerSmoke.Emit(100); Spawner.RemoveOnLane(WhichLane, PlaceInLane); GameManager.instance.tazdokHp--; };
        loop += Movement;
        loop += Rage;
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

        if (pita.Count == Order.Count) // if the pita and the order contains the same amount
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
            done= true;
        }
        else
        {
            CurrentRage += 10;
        }
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
            RandomOrder.Add((BuildOrder.Fillers)UnityEngine.Random.Range(0,count));
        }

        Order = RandomOrder;
    }
}
