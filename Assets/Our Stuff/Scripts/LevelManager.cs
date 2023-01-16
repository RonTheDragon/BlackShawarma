using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<SOLevel> _levels = new List<SOLevel>();

    [SerializeField] private EnemySpawner _spawner;
    private LevelTimer _timer => GetComponent<LevelTimer>();

    private int _currentLevel;

    // Start is called before the first frame update
    private void Start()
    {
        SetUpLevel(_levels[_currentLevel]);
    }

    private void SetUpLevel(SOLevel lvl)
    {
        _spawner.LevelSetUp(lvl.Enemies, lvl.RandomSpawnRate, lvl.WarmUpTime, lvl.MaxEnemiesAtOnce);
        _timer.StartTimer(lvl.Seconds, lvl.Minutes);
    }

    public void NextLevel()
    {
        if (_currentLevel < _levels.Count-1) _currentLevel++; 
        else { Debug.Log("Out of Levels"); }

        SetUpLevel(_levels[_currentLevel]);
    }

    public void RepeatLevel()
    {
        SetUpLevel(_levels[_currentLevel]);
    }
}
