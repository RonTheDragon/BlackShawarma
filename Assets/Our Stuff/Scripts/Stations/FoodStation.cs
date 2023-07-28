using System;
using UnityEngine;

public class FoodStation : MonoBehaviour , HoldInteractable
{
    [SerializeField]
    private string _info;

    [SerializeField]
    private string _refillInfo;

    private string _useInfo;
    public string Info { get => _info; set => _info = value; }

    private bool _notActive;
    public bool NotActive { get => _notActive; set => _notActive = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    [SerializeField] GameObject Panel;

    private Gun _gun;

    [SerializeField] private bool _holdToUse;
    public bool HoldToUse { get => _holdToUse; set => _holdToUse = value; }

    [SerializeField] private float _useDuration;
    public float UseDuration { get => _useDuration; set => _useDuration = value; }

    private GameManager _gm;
    private AudioManager _am;

    public string NotActiveInfo { get => _notActiveInfo; set => _notActiveInfo = value; }
    private string _notActiveInfo;

    private void Start()
    {
        _am = AudioManager.instance;
        _gm = GameManager.Instance;
        _useInfo = _info;
        _gm.OnPickUpSack += () => ToggleRefill(true);
        _gm.OnPlaceDownSack += () => ToggleRefill(false);
    }

    public void Use(GameObject player)
    {
        _used?.Invoke();
        _gun = player.GetComponent<Gun>();
        BuildOrder b = player.GetComponent<BuildOrder>();

        

        if (!b.HasSupplies)
        {
            Panel.SetActive(!Panel.activeSelf);
            
            if (_gun != null)
            {
                
                if (!_gun.UsingUI)
                {
                    _gun.StartUsingStation();
                   _am.PlayOneShot(FMODEvents.instance.Counter, transform.position);
                }
                else
                {
                    _gun.StopUsingStation();
                    _gun = null;
                }
            }
        }
        else
        {
            b.FillAll();
            b.HasSupplies = false;
            b.Sack.SetActive(false);
            _gm.OnPlaceDownSack?.Invoke();
        }
    }

    private void ToggleRefill(bool toggle)
    {
        if (toggle) 
        {
            _info = _refillInfo;
            _holdToUse = true;
        }
        else
        {
            _info = _useInfo;
            _holdToUse = false;
        }
    }
    public void UpdateInfo()
    {

    }
}
