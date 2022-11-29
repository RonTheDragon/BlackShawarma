using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent=> GetComponent<NavMeshAgent>();
    [SerializeField] float CurrentRage, MaxRage;
    event EventHandler OnRage;
    Vector3 destination;
    public int PlaceInLane;
    void Start()
    {
        
    }

    
    void Update()
    {
        Rage();
        Movement();
    }


    void Movement()
    {
        agent.SetDestination(destination);
    }
    void Rage()
    {
        if(CurrentRage < MaxRage)
        {
            CurrentRage += Time.deltaTime;
        }
        else
        {
            OnRage?.Invoke(this, EventArgs.Empty);
            CurrentRage = 0;
        }
    }

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
    }
}
