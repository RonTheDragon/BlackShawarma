using System;
using Unity.VisualScripting;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private GameManager _gm;

    private int _combo;
    private float _comboTimeLeft;
    [SerializeField] private float _comboResetTime = 15;
    [SerializeField] private int _maxComboBenefit=10;
    [SerializeField] private int _minComboBenefit = 2;
    [SerializeField] private float _comboBenefit=0.1f;

    public Action<int> AddEvent;
    public Action ResetEvent;
    public Action<float> TimerEvent;


    // Start is called before the first frame update
    private void Start()
    {
        _gm = GameManager.Instance;
        ResetCombo();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_combo > 0)
        {
            _comboTimeLeft -= Time.deltaTime;
            TimerEvent?.Invoke(_comboTimeLeft / _comboResetTime);
            if ( _comboTimeLeft <= 0 ) { ResetCombo(); }
        }
    }

    public void ResetCombo()
    {
        _combo = 0;
        _gm.SetMoneyMultiplier(1);
        ResetEvent?.Invoke();
        //Debug.Log("Combo Reset");
    }

    public void AddCombo()
    {
        _gm.HappyCustomers++;
        _combo++;
        _comboTimeLeft = _comboResetTime;
        if (_combo >= _minComboBenefit)
        {
            AddEvent?.Invoke(_combo);
            float benefit = _comboBenefit;
            if ( benefit > _maxComboBenefit ) benefit = _maxComboBenefit;
            _gm.SetMoneyMultiplier(1 + (_combo * benefit));
        }
        //Debug.Log($"Combo: {_combo}");
    }


}
