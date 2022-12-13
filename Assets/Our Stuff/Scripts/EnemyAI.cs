using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    bool done;


    public void Spawn(EnemySpawner spawner)
    {
        Spawner = spawner;
        done = false;
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
        }
    }

    public void Eat(Adible.Food f)
    {
        if (CaniEAT((int)f))
        {
            CurrentRage -= 10;
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
    bool CaniEAT(int a)
    {
        bool _didIEatit = false;
        switch (a)
        {
            case 0:
                _didIEatit = _falefelEater;
                break;
            case 1:
                _didIEatit = _eggplantEater;
                break;
            case 2:
                _didIEatit = _friesEater;
                break;
            default:
                break;
        }
        return _didIEatit;
    }
}
