using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour 
{
    NavMeshAgent agent=> GetComponent<NavMeshAgent>();
    [SerializeField] float CurrentRage, MaxRage;
    Action OnRage;
    Vector3 destination;
    public int PlaceInLane;

    [SerializeField] float AngerSmokeAmount=1;
    [SerializeField] ParticleSystem AngerSmoke;

    EnemySpawner Spawner;

    Action loop;

    bool done;

    public void Spawn(EnemySpawner spawner)
    {
        Spawner = spawner;
        done = false;
    }
    

    void Start()
    {
        OnRage += () => AngerSmoke.Emit(100);
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
        emission.rateOverTime = CurrentRage*0.1f*AngerSmokeAmount;
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
            done = true;
            CurrentRage = 0;

        }
    }

    public void Eat()
    {
        CurrentRage -= 10;
    }

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
    }
}
