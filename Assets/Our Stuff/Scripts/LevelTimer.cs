using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public int Seconds, Minutes;

    private float second;

    private Action loop;

    public Action<int, int> OnUpdateTimer;

    public Action OnTimerDone;
    

    // Start is called before the first frame update
    void Start()
    {
        StartTimer(10, 1);

        OnTimerDone = () => Debug.Log("Timer Done");
    }

    // Update is called once per frame
    void Update()
    {
        loop?.Invoke();
    }

    void RunTimer()
    {
        if (Seconds <= 0)
        {
            if (Minutes<=0) { loop -= RunTimer; OnTimerDone?.Invoke();  return; }
            Minutes--;
            Seconds = 60;
        }
        OneSecond();   
    }

    void OneSecond()
    {
        second -= Time.deltaTime;
        if (second <= 0)
        {
            Seconds--;
            second = 1;
            OnUpdateTimer?.Invoke(Seconds,Minutes);
        }
    }

    public void StartTimer(int S, int M = 0)
    {
        Minutes  = 0;
        Seconds  = 0;
        Minutes += (S / 60) + M; // add minutes from the seconds
        Seconds  = (S + 1) % 60; // Clean the seconds from minutes
        loop    += RunTimer;
    }
}
