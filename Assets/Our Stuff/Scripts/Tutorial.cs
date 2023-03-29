using System;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // UI
    [SerializeField] private Transform TutorialPanel;
    private Transform _skipTutorialPanel => TutorialPanel.Find("SkipTutorialPanel");
    private Transform _tutorialStage1 => TutorialPanel.Find("RefillAmmo");


    // refs
    private GameManager _gm => GetComponent<GameManager>();
    private Gun _gun => _gm.Player.GetComponent<Gun>();
    private List<SOAmmoType> _ammoTypes => _gun.AmmoTypes;
    private LevelManager _levelManager => GetComponent<LevelManager>();
    private EnemySpawner _enemySpawner => _gm.EnemySpawner;

    private EnemyAI _enemy;


    // Events
    private Action _currentTutorialStage; // Current State Used
    public Action<bool> FreezeTimer;

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

    public void StartTutorial()
    {
        Time.timeScale = 1.0f;
        _skipTutorialPanel.gameObject.SetActive(false);
        _currentTutorialStage = TutorialStage1;
        _gun.StopUsingStation();

        // Tutorial Start:
        _tutorialStage1.gameObject.SetActive(true);
        foreach (SOAmmoType ammoType in _ammoTypes)
        {
            ammoType.CurrentAmmo = 0;
        }
        FreezeTimer?.Invoke(true);
    }

    public void SkipTutorial()
    {
        Time.timeScale = 1.0f;
        _skipTutorialPanel.gameObject.SetActive(false);
        _levelManager.NextLevel();
        _gun.StopUsingStation();
    }

    #region TutorialStages
    private void TutorialStage1()
    {
        // Update Content Here

        if (IsAllAmmoRefilled()) // Finish Stage Requirement
        {
            _tutorialStage1.gameObject.SetActive(false);
            _enemySpawner.ChangeMaxEnemiesInGame(1);
            StartCoroutine("GetEnemy");
            FreezeTimer?.Invoke(false);
            
            _currentTutorialStage = TutorialStage2;
        }
    }

    private void TutorialStage2()
    {

    }
    #endregion

    #region TutorialIfs
    private bool IsAllAmmoRefilled()
    {
        foreach (SOAmmoType ammoType in _ammoTypes)
        {
            if (ammoType.CurrentAmmo == 0)
                return false;
        }
        return true;
    }


    #endregion

    System.Collections.IEnumerator GetEnemy()
    {
        yield return new WaitForSeconds(1.2f);
        _enemy = _enemySpawner.GetFirstEnemy();
        _enemy.RageMultiplier = 0;
    }
}
