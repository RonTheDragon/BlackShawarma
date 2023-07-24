using System;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public float TimeLeft;

    public Action<float> OnSetTimer;

    public Action OnTimerDone;
    private bool _bIsDone = false;

    private GameManager _gm => GameManager.Instance;
    

    // Start is called before the first frame update
    void Start()
    {
        OnTimerDone += () => { Debug.Log("Timer Done"); _bIsDone = true; _gm.DidWeWin(); };
    }

    public void StartTimer(float time)
    {
        _bIsDone    = false;
        TimeLeft= time;
        OnSetTimer?.Invoke(time);
        Time.timeScale = 1;
        _gm.CM.ResetCombo();
    }

    public void SetTimerTo0()
    {
        TimeLeft = 0;
    }

    public bool GetIsTimeDone()
    {
        return _bIsDone;
    }
}
