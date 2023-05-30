using System;
using UnityEngine;

public class Refill : MonoBehaviour, Interactable
{
    [SerializeField] SOAmmoType _type;
    [SerializeField] private string _info;

    [SerializeField] private bool UseOption1;

    [Header("Option 1")]
    [SerializeField] private float _turnOffDistance = 3;
    [SerializeField] private int _perHarvest = 5;
    [SerializeField] private float _harvestCooldown = 1;
    private float _currentHarvestCooldown;

    [Header("Option 2")]
    [SerializeField] private float _cookingDuration = 3;
    private float _currentCooking;
    private bool _cooking;
    private bool _filled;
    public string Info { get => _info; set => _info = value; }

    private bool _active;

    public bool NotActive { get => _active; set => _active = value; }
    private Camera PlayerCamera => Camera.main;

    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    [Header("refrences")]
    [SerializeField] private GameObject _ui;
    [SerializeField] private Animator _anim;

    private Transform _playerLocation;
    private Gun _gun;

    private bool _refillerWorking;

    private SOAmmoType _selectedFood;

    private void Update()
    {
        if (UseOption1)
        {
            Working();
        }
        else
        {
            Working2();
        }
    }

    public void Use(GameObject player)
    {
        _used?.Invoke();
        if (UseOption1)
        {
            Option1(player);
        }
        else
        {
            Option2(player);
        }
    }

    private void NeedsRefill()
    {
        if (_type.CurrentAmmo == 0)
        {
            if (_ui.activeSelf == false) _ui.SetActive(true);
            _ui.transform.LookAt(PlayerCamera.transform.position);
        }
        else if (_ui.activeSelf == true)
        {
            _ui.SetActive(false);
        }
    }

    private void NeedsRefill2()
    {
        if (_type.CurrentAmmo == 0 || _filled)
        {
            if (_ui.activeSelf == false) _ui.SetActive(true);
            _ui.transform.LookAt(PlayerCamera.transform.position);
        }
        else if (_ui.activeSelf == true)
        {
            _ui.SetActive(false);
        }
    }

    private void Option1(GameObject player)
    {
        if (!_refillerWorking)
        {
            _playerLocation = player.transform;
            _gun = player.GetComponent<Gun>();
            _currentHarvestCooldown = _harvestCooldown;
            _refillerWorking = true;
            _anim.SetBool("Active", true);
            foreach (SOAmmoType i in _gun.AmmoTypes)
            {
                if (i.FoodType == _type.FoodType)
                {
                    _selectedFood = i; break;
                }
            }
        }
    }

    private void Option2(GameObject player)
    {
        _gun = player.GetComponent<Gun>();
        foreach (SOAmmoType i in _gun.AmmoTypes)
        {
            if (i.FoodType == _type.FoodType)
            {
                _selectedFood = i; break;
            }
        }

        if (_filled) 
        {
            _filled = false;
            _selectedFood.CurrentAmmo = _selectedFood.MaxAmmo;
            return;
        }
        else 
        {
            if (!_cooking)
            {
                _anim.SetBool("Active", true); _cooking = true; _currentCooking = _cookingDuration;
            }
        }
    }

    private void Working()
    {
        NeedsRefill();

        if (!_refillerWorking) return;

        if (Vector3.Distance(transform.position, _playerLocation.position)> _turnOffDistance) { _refillerWorking = false; _anim.SetBool("Active", false); return; }

        _currentHarvestCooldown -= Time.deltaTime;

        if (_selectedFood.CurrentAmmo == _selectedFood.MaxAmmo) {  _refillerWorking = false; _anim.SetBool("Active", false); return; }

        if (_currentHarvestCooldown < 0) 
        { 
            _currentHarvestCooldown = _harvestCooldown;
            _selectedFood.CurrentAmmo += _perHarvest;
            if (_selectedFood.CurrentAmmo> _selectedFood.MaxAmmo) { _selectedFood.CurrentAmmo = _selectedFood.MaxAmmo; }
        }
    }

    private void Working2()
    {
        NeedsRefill2();
        if (_cooking)
        {
            _currentCooking -= Time.deltaTime;
            if ( _currentCooking < 0)
            {
                _filled = true; _cooking = false; _anim.SetBool("Active", false);
            }
        }
    }
}
