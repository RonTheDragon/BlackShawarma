using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Coffee : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
                     private Gun    _gun;
    private ThirdPersonMovement _tpm;
                     public  int    coffeeBuffTime   = 10;
    [SerializeField] private int _defaultCooldown = 10;
    [SerializeField] private float  _cooldown        = 0;
    [SerializeField] private GameObject _ui;
    private Camera PlayerCamera => Camera.main;
    private Image _fillable => _ui.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
    public string Info { get => _info; set => _info = value; }

    private bool _notActive;
    public bool NotActive { get => _notActive; set => _notActive = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    private void Start()
    {
        gameObject.SetActive(false);
        _cooldown = 0;
    }

    private void Update()
    {
        stationUI();
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
        else if (_cooldown < 0)
        {
            _cooldown = 0;
            _notActive = false;
            _fillable.fillAmount = 1;
        }
    }

    public void Use(GameObject player)
    {
        if (_cooldown > 0)
        {
            return;
        }
        _used?.Invoke();
        if (_gun == null)
        {
            _gun = player.GetComponent<Gun>();
            _tpm = player.GetComponent<ThirdPersonMovement>();
        }
        _gun.SetCoffee(true);
        _tpm.SetCoffee(true);
        Invoke("CoffeeBuffHandler", coffeeBuffTime);
        _cooldown = _defaultCooldown+ coffeeBuffTime;
        _notActive = true;
    }

    private void CoffeeBuffHandler()
    {
        _gun.SetCoffee(false);
        _tpm.SetCoffee(false);
    }

    public void UpgradeCoffee(int buffDuration, int cooldown)
    {
        coffeeBuffTime = buffDuration;
        _defaultCooldown = cooldown;
    }

    private void stationUI()
    {
        if (PlayerCamera == null) return;
        _ui.transform.LookAt(PlayerCamera.transform.position);
        if (_cooldown>0)
        {
            _fillable.fillAmount = 1 - (_cooldown / (_defaultCooldown + coffeeBuffTime));
        }
    }
}