using System;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // UI
    [SerializeField] private Transform TutorialPanel;
    private Transform _skipTutorialPanel => TutorialPanel.Find("SkipTutorialPanel");


    // refs
    [SerializeField] private GameObject _player;
    private Gun _gun => _player.GetComponent<Gun>();
    private LevelManager _levelManager => GetComponent<LevelManager>();


    // Events
    private Action _currentTutorialStage; // Current State Used

    private void Start()
    {
        _skipTutorialPanel.gameObject.SetActive(true);
        _gun.StartUsingStation();
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
    }

    public void SkipTutorial()
    {
        Time.timeScale = 1.0f;
        _skipTutorialPanel.gameObject.SetActive(false);
        _levelManager.NextLevel();
        _gun.StopUsingStation();
    }
    

    private void TutorialStage1()
    {
        // Update Content Here

        if (true) // Finish Stage Requirement
        {
            _currentTutorialStage = TutorialStage2;
        }
    }

    private void TutorialStage2()
    {

    }
}
