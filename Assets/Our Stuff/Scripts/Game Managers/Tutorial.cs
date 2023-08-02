using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    // UI
    [SerializeField] private Transform TutorialPanel;
    //private Transform _skipTutorialPanel => TutorialPanel.Find("SkipTutorialPanel");
    //private Transform _tutorialStage1 => TutorialPanel.Find("MeetOlga");
    //private Transform _tutorialStage2 => TutorialPanel.Find("RefillAmmo");
    //private Transform _tutorialStage3 => TutorialPanel.Find("Tab and Rage");
    //private Transform _tutorialStage4 => TutorialPanel.Find("ShootEnemy");
    //private Transform _tutorialStage5 => TutorialPanel.Find("OpenEmptyCounter");
    //private Transform _tutorialStage6 => TutorialPanel.Find("RefillSupplies");
    //private Transform _tutorialStage7 => TutorialPanel.Find("BuildOrder");
    //private Transform _tutorialStage8 => TutorialPanel.Find("CalmHimAgain");
    //private Transform _tutorialStage9 => TutorialPanel.Find("ShootShawarma");
    //private Transform _tutorialStage10 => TutorialPanel.Find("GoodJob");

    private List<Transform> _tutorialStages = new List<Transform>();

    //UI image

    [SerializeField] private GameObject[] FoodImages;

    // refs

    [SerializeField] private Supplies _suppliesInteractable;
    [SerializeField] private FoodStation _counterInteractable;
    private GameManager _gm => GetComponent<GameManager>();
    private Gun _gun => _gm.Player.GetComponent<Gun>();
    private BuildOrder _bo => _gm.Player.GetComponent<BuildOrder>();
    private List<SOAmmoType> _ammoTypes => _gun.AmmoTypes;
    private LevelManager _levelManager => GetComponent<LevelManager>();
    private EnemySpawner _enemySpawner => _gm.EnemySpawner;
    private Transform _tutorialPointer => transform.Find("TutorialPointer");

    private EnemyAI _enemy;

    private int currentStep = 0;

    [SerializeField] private EventReference[] tutorialEvents;

    [SerializeField] private FMODEvents fmodEvents;

    private StudioEventEmitter tutorialEmitter;



    // Events
    private Action _currentTutorialStage; // Current State Used
    public Action<bool> FreezeTimer;

    private bool _done;
    private bool isPlayingSound;

    private void Start()
    {
        foreach (Transform tutorial in TutorialPanel)
        {
            _tutorialStages.Add(tutorial);
        }
        _tutorialStages[0].gameObject.SetActive(true);
        _gun.StartUsingStation();
        Time.timeScale = 0.0f;
        StartCoroutine("lateStart");
        tutorialEmitter = GetComponent<StudioEventEmitter>();
        PlayNextSound();


    }


    System.Collections.IEnumerator lateStart()
    {
        yield return null;
        yield return null;
        yield return null;
        Time.timeScale = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        _currentTutorialStage?.Invoke();
        if (isPlayingSound)
        {
            // If the current sound is not playing anymore
            if (!tutorialEmitter.IsPlaying())
            {
                // Sound has finished playing, move to the next step
                currentStep++;

                // Set the flag to indicate that no sound is playing now
                isPlayingSound = false;

                // Play the sound for the next step
                PlayNextSound();
            }
        }

    }
    public void SkipTutorial()
    {
        Time.timeScale = 1.0f;
        _tutorialStages[0].gameObject.SetActive(false);
        _levelManager.NextLevel();
        _gun.StopUsingStation();
    }

    public void StartTutorial()
    {
        Time.timeScale = 1.0f;
        _tutorialStages[0].gameObject.SetActive(false);
        _currentTutorialStage = TutorialStage1;
        _gun.StopUsingStation();

        // Tutorial Start:
        foreach (SOAmmoType ammoType in _ammoTypes)
        {
            ammoType.CurrentAmmo = 0;
        }

        _gm.OnAmmoUpdate?.Invoke();
        FreezeTimer?.Invoke(true);
        _tutorialStages[1].gameObject.SetActive(true);
        _counterInteractable.NotActive = true;
        _bo.EmptyAll();
        _suppliesInteractable.NotActive = true;
        _gun.CantUseStations = true;

        

    }


    #region TutorialStages

    private void TutorialStage1() //movement
    {

        // Update Content Here
        if (Input.GetKeyDown(KeyCode.Space)) // Finish Stage Requirement
        {
            // Move to the next step


            // Play the sound for the next step

            _currentTutorialStage = TutorialStage2;
            _tutorialStages[1].gameObject.SetActive(false);
            _tutorialStages[2].gameObject.SetActive(true);
        }
    }

    private void TutorialStage2() //Mouse
    {
        // Update Content Here
        if (Input.GetKeyDown(KeyCode.Space)) // Finish Stage Requirement
        {
            _currentTutorialStage = TutorialStage3;
            _tutorialStages[2].gameObject.SetActive(false);
            _tutorialStages[3].gameObject.SetActive(true);
        }
    }

    private void TutorialStage3() //Meet Olga
    {
        // Update Content Here
        if (Input.GetKeyDown(KeyCode.Space)) // Finish Stage Requirement
        {
            _currentTutorialStage = TutorialStage4;
            _tutorialStages[3].gameObject.SetActive(false);
            _tutorialStages[4].gameObject.SetActive(true);
            _gun.CantUseStations = false;
            foreach (GameObject image in FoodImages)
            {
                image.gameObject.SetActive(true);
            }
        }
    }

    private void TutorialStage4() // refill
    {
        // Update Content Here

        if (IsAllAmmoRefilled()) // Finish Stage Requirement
        {
            _tutorialStages[4].gameObject.SetActive(false);
            _tutorialStages[5].gameObject.SetActive(true);
            _enemySpawner.ChangeMaxEnemiesInGame(1);
            StartCoroutine("GetEnemy");

            _currentTutorialStage = TutorialStage5;
        }
    }

    //private void TutorialStage5() //tab and rage
    //{
    //    // Update Content Here
    //    if (Input.GetKeyDown(KeyCode.Space)) // Finish Stage Requirement
    //    {
    //        _currentTutorialStage = TutorialStage6;
    //        _tutorialStages[5].gameObject.SetActive(false);
    //        _tutorialStages[6].gameObject.SetActive(true);
    //    }
    //}

    private void TutorialStage5() // shoot him calm
    {
        if (_enemy == null) return;

        _tutorialPointer.gameObject.SetActive(true);
        _tutorialPointer.position = _enemy.transform.position + Vector3.up * 3;


        if (_enemy.CalmEnoughToEat()) // Finish Stage Requirement
        {
            _tutorialStages[5].gameObject.SetActive(false);
            _tutorialStages[6].gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage6;
            _counterInteractable.NotActive = false;
            _done = false;
            _counterInteractable.Used += EventDone;
            _tutorialPointer.position = _counterInteractable.transform.position + Vector3.up * 2;
        }
    }

    private void TutorialStage6() // Open Empty Counter
    {

        if (_done) // Finish Stage Requirement
        {
            // Update Content Here

            _done = false;
            _suppliesInteractable.NotActive = false;
            _suppliesInteractable.Used += EventDone;
            _counterInteractable.Used -= EventDone;
            _tutorialStages[6].gameObject.SetActive(false);
            _tutorialStages[7].gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage7;
            _tutorialPointer.position = _suppliesInteractable.transform.position + Vector3.up * 3;
        }
    }

    private void TutorialStage7() // Get Supplies
    {
        // Update Content Here

        // TO DO: The tutorial goes to the next part the moment the player picks up a new sack,
        // instead of after the player refills the counter.

        if (_done) // Finish Stage Requirement
        {
            _done = false;
            _suppliesInteractable.Used -= EventDone;
            _tutorialStages[7].gameObject.SetActive(false);
            _tutorialStages[8].gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage8;
            _tutorialPointer.position = _counterInteractable.transform.position + Vector3.up * 2;
        }
    }

    private void TutorialStage8() // Build The Shawarma
    {
        // Update Content Here

        if (_enemy.CheckIfPitaCorrect(_gun.GetPita())) // Finish Stage Requirement
        {
            _tutorialStages[8].gameObject.SetActive(false);
            _tutorialStages[9].gameObject.SetActive(true);
            _enemy.SetCurrentRage(50);
            _currentTutorialStage = TutorialStage9;
            _tutorialPointer.position = _enemy.transform.position + Vector3.up * 3;
        }
    }

    private void TutorialStage9() // Calm Him Again
    {
        // Update Content Here

        if (_enemy.CalmEnoughToEat()) // Finish Stage Requirement
        {
            _tutorialStages[9].gameObject.SetActive(false);
            _tutorialStages[10].gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage10;
            _enemySpawner.ChangeMaxEnemiesInGame(0);
        }
    }
    private void TutorialStage10() // Shoot The Shawarma
    {
        // Update Content Here

        if (_enemy.SideOrder == null) // Finish Stage Requirement
        {
            _tutorialStages[10].gameObject.SetActive(false);
            _tutorialStages[11].gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage11;
            _tutorialPointer.gameObject.SetActive(false);
        }
    }

    private void TutorialStage11() // Build The Shawarma
    {
        // Update Content Here

        if (!_enemy.gameObject.activeSelf) // Finish Stage Requirement
        {
            _tutorialStages[11].gameObject.SetActive(false);
            _currentTutorialStage = null;
            FreezeTimer?.Invoke(false);
            _gm.GetComponent<LevelTimer>().SetTimerTo0();
        }
    }
    #endregion

    #region TutorialIfs
    private bool IsAllAmmoRefilled()
    {
        bool allFoodIsFilled = true;
        for (int i = 0; i < 3; i++)
        {
            if (_ammoTypes[i].CurrentAmmo == 0)
            {
                allFoodIsFilled = false;
            }
            else
            {
                FoodImages[i].gameObject.SetActive(false);

            }

        }
        return allFoodIsFilled;
    }


    #endregion

    System.Collections.IEnumerator GetEnemy()
    {
        yield return new WaitForSeconds(1.2f);
        _enemy = _enemySpawner.GetFirstEnemy();
        _enemy.LevelRageMultiplier = 0;
    }

    private void EventDone()
    {
        _done = true;
    }
    private void PlayNextSound()
    {
        if (currentStep < tutorialEvents.Length)
        {
            // If a sound is already playing, stop it before playing the next sound
            if (isPlayingSound)
            {
                tutorialEmitter.Stop();
            }

            // Get the event reference for the current step
            EventReference currentEvent = tutorialEvents[currentStep];

            // Assign the event reference to the StudioEventEmitter
            tutorialEmitter.EventReference = currentEvent;

            // Play the current step sound
            tutorialEmitter.Play();

            // Set a flag to indicate that sound is playing
            isPlayingSound = true;
        }
    }

}
