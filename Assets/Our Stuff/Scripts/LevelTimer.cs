using System;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public float TimeLeft;

    public Action<float> OnSetTimer;

    public Action OnTimerDone;
    public bool IsDone = false;

    private GameManager _gm => GameManager.Instance;
    

    // Start is called before the first frame update
    void Start()
    {
        OnTimerDone += () => { Debug.Log("Timer Done"); IsDone = true; _gm.DidWeWin(); };
    }

    public void StartTimer(float time)
    {
        IsDone    = false;
        TimeLeft= time;
        OnSetTimer?.Invoke(time);
    }

    public void SetTimerTo0()
    {
        TimeLeft = 0;
    }
}
