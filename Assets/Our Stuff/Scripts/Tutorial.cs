using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private Action _currentTutorialStage; // Current State Used
    private bool _started; // makes every State have a start functionality

    // Update is called once per frame
    void Update()
    {
        _currentTutorialStage?.Invoke();
    }

    public void StartTutorial()
    {
        StartStage(TutorialStage1);
    }

    // Used To Start a Tutorial Stage
    private void StartStage(Action Stage)
    {
        _started = false;
        _currentTutorialStage = Stage;
    }
    

    private void TutorialStage1()
    {
        if (!_started) 
        { 
            _started = true;
            // Start Content Here
        }
            // Update Content Here


        if (true) // Finish Stage Requirement
        {
            StartStage(TutorialStage2);
        }
    }

    private void TutorialStage2()
    {

    }
}
