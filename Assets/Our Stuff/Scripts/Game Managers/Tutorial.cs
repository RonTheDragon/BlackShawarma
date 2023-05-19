using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FMODUnity;

public class Tutorial : MonoBehaviour
{
    // UI
    [SerializeField] private Transform TutorialPanel;
    private Transform _skipTutorialPanel => TutorialPanel.Find("SkipTutorialPanel");
    private Transform _tutorialStage1 => TutorialPanel.Find("MeetOlga");
    private Transform _tutorialStage2 => TutorialPanel.Find("RefillAmmo");
    private Transform _tutorialStage3 => TutorialPanel.Find("Tab and Rage");
    private Transform _tutorialStage4 => TutorialPanel.Find("ShootEnemy");
    private Transform _tutorialStage5 => TutorialPanel.Find("OpenEmptyCounter");
    private Transform _tutorialStage6 => TutorialPanel.Find("RefillSupplies");
    private Transform _tutorialStage7 => TutorialPanel.Find("BuildOrder");
    private Transform _tutorialStage8 => TutorialPanel.Find("CalmHimAgain");
    private Transform _tutorialStage9 => TutorialPanel.Find("ShootShawarma");
    private Transform _tutorialStage10 => TutorialPanel.Find("GoodJob");
    
    //UI image
    
    [SerializeField] private GameObject[] FoodImages ;
    
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


    // Events
    private Action _currentTutorialStage; // Current State Used
    public Action<bool> FreezeTimer;

    private float _waiting = 0;
    private bool _done;

    private void Start()
    {
        _skipTutorialPanel.gameObject.SetActive(true);
        _gun.StartUsingStation();
        Time.timeScale = 0.0f;
        StartCoroutine("lateStart");
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

       
    }
    public void SkipTutorial()
    {
        Time.timeScale = 1.0f;
        _skipTutorialPanel.gameObject.SetActive(false);
        _levelManager.NextLevel();
        _gun.StopUsingStation();
    }

    public void StartTutorial()
    {
        Time.timeScale = 1.0f;
        _skipTutorialPanel.gameObject.SetActive(false);
        _currentTutorialStage = TutorialStage1;
        _gun.StopUsingStation();

        // Tutorial Start:
        foreach (SOAmmoType ammoType in _ammoTypes)
        {
            ammoType.CurrentAmmo = 0;
        }
        FreezeTimer?.Invoke(true);
        _waiting = 0;
        _tutorialStage1.gameObject.SetActive(true);
        _counterInteractable.NotActive = true;
        _bo.EmptyAll();
        _suppliesInteractable.NotActive = true;
        _gun.CantUseStations = true;
    }


    #region TutorialStages

    private void TutorialStage1() //meet olga
    {
        // Update Content Here
        _waiting += Time.deltaTime;


        if (_waiting > 5) // Finish Stage Requirement
        {
            _waiting = 0;
            _gun.CantUseStations = false;
            _currentTutorialStage = TutorialStage2;
            _tutorialStage1.gameObject.SetActive(false);
            _tutorialStage2.gameObject.SetActive(true);
            foreach (GameObject image in FoodImages)
            {
                image.gameObject.SetActive(true);
            }
        }
    }

    private void TutorialStage2() // refill
    {
        // Update Content Here

        if (IsAllAmmoRefilled()) // Finish Stage Requirement
        {
            _tutorialStage2.gameObject.SetActive(false);
            _tutorialStage3.gameObject.SetActive(true);
            _enemySpawner.ChangeMaxEnemiesInGame(1);
            StartCoroutine("GetEnemy");
            

            _currentTutorialStage = TutorialStage3;
        }
    }

    private void TutorialStage3() // meet rage system and tab
    {
        // Update Content Here
        _waiting += Time.deltaTime;


        if (_waiting > 5) // Finish Stage Requirement
        {
            _waiting = 0;
            _tutorialStage3.gameObject.SetActive(false);
            _tutorialStage4.gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage4;
            _tutorialPointer.gameObject.SetActive(true);
            _tutorialPointer.position = _enemy.transform.position + Vector3.up * 3;
           
        }
    }

    private void TutorialStage4() // shoot him calm
    {
        // Update Content Here



        if (_enemy.CalmEnoughToEat()) // Finish Stage Requirement
        {
            _counterInteractable.NotActive = false;
            _tutorialStage4.gameObject.SetActive(false);
            _tutorialStage5.gameObject.SetActive(true);
            _done = false;
            _counterInteractable.Used += EventDone;
            _currentTutorialStage = TutorialStage5;
            _tutorialPointer.position = _counterInteractable.transform.position + Vector3.up * 2;
        }
    }

    private void TutorialStage5() // Open Empty Counter
    {
        // Update Content Here



        if (_done) // Finish Stage Requirement
        {
            _done = false;
            _suppliesInteractable.NotActive = false;
            _suppliesInteractable.Used += EventDone;
            _counterInteractable.Used -= EventDone;
            _tutorialStage5.gameObject.SetActive(false);
            _tutorialStage6.gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage6;
            _tutorialPointer.position = _suppliesInteractable.transform.position + Vector3.up * 3;
        }
    }

    private void TutorialStage6() // Get Supplies
    {
        // Update Content Here



        if (_done) // Finish Stage Requirement
        {
            _done = false;
            _suppliesInteractable.NotActive = false;
            _suppliesInteractable.Used -= EventDone;
            _tutorialStage6.gameObject.SetActive(false);
            _tutorialStage7.gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage7;
            _tutorialPointer.position = _counterInteractable.transform.position + Vector3.up * 2;
        }
    }

    private void TutorialStage7() // Build The Shawarma
    {
        // Update Content Here



        if (_enemy.CheckIfPitaCorrect(_gun.GetPita())) // Finish Stage Requirement
        {
            _tutorialStage7.gameObject.SetActive(false);
            _tutorialStage8.gameObject.SetActive(true);
            _enemy.SetCurrentRage(50);
            _currentTutorialStage = TutorialStage8;
            _tutorialPointer.position = _enemy.transform.position + Vector3.up * 3;
        }
    }

    private void TutorialStage8() // Calm Him Again
    {
        // Update Content Here



        if (_enemy.CalmEnoughToEat()) // Finish Stage Requirement
        {
            _tutorialStage8.gameObject.SetActive(false);
            _tutorialStage9.gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage9;
            _enemySpawner.ChangeMaxEnemiesInGame(0);
        }
    }
    private void TutorialStage9() // Shoot The Shawarma
    {
        // Update Content Here



        if (_enemy.SideOrder == null) // Finish Stage Requirement
        {
            _tutorialStage9.gameObject.SetActive(false);
            _tutorialStage10.gameObject.SetActive(true);
            _currentTutorialStage = TutorialStage10;
            _tutorialPointer.gameObject.SetActive(false);
        }
    }

    private void TutorialStage10() // Build The Shawarma
    {
        // Update Content Here



        if (!_enemy.gameObject.activeSelf) // Finish Stage Requirement
        {
            _tutorialStage10.gameObject.SetActive(false);
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
}
