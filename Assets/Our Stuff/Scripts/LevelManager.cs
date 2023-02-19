using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<SOLevel> _levels = new List<SOLevel>();

    [SerializeField] private EnemySpawner _spawner;
    private LevelTimer _timer => GetComponent<LevelTimer>();

    private int _currentLevel;

    public Action OnSetUpLevel;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine("lateStart");
        //SetUpLevel(_levels[_currentLevel]);
    }

    System.Collections.IEnumerator lateStart()
    {
        yield return null;
        yield return null;
        SetUpLevel(_levels[_currentLevel]);
    }

    private void SetUpLevel(SOLevel lvl)
    {
        _spawner.LevelSetUp(lvl.Enemies, lvl.RandomSpawnRate, lvl.WarmUpTime, lvl.MaxEnemiesAtOnce);
        _timer.StartTimer(lvl.Seconds);
        OnSetUpLevel?.Invoke();
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
