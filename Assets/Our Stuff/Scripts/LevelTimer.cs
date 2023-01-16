using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEditorInternal;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    private int _seconds, _minutes;

    private float _second;

    private Action _loop;

    public Action<int, int> OnUpdateTimer;

    public Action OnTimerDone;
    public bool IsDone = false;

    private GameManager _gm => GameManager.instance;
    

    // Start is called before the first frame update
    void Start()
    {
        //StartTimer(3, 0);
        
        OnTimerDone += () => { Debug.Log("Timer Done"); IsDone = true; _gm.DidWeWin(); };
    }

    // Update is called once per frame
    void Update()
    {
        _loop?.Invoke();
    }

    void RunTimer()
    {
        if (_seconds <= 0)
        {
            if (_minutes<=0) { _loop -= RunTimer; OnTimerDone?.Invoke();  return; }
            _minutes--;
            _seconds = 60;
        }
        OneSecond();   
    }

    void OneSecond()
    {
        _second -= Time.deltaTime;
        if (_second <= 0)
        {
            _seconds--;
            _second = 1;
            OnUpdateTimer?.Invoke(_seconds,_minutes);
        }
    }

    public void StartTimer(int S, int M = 0)
    {
        IsDone = false;
        _minutes  = 0;
        _seconds  = 0;
        _minutes += (S / 60) + M; // add minutes from the seconds
        _seconds  = (S + 1) % 60; // Clean the seconds from minutes
        _loop    += RunTimer;
    }
    
}
