using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coffee : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
                     private Gun    _gun;
                     public  int    coffeeBuffTime   = 10;
    [SerializeField] private int    _defaultCooldown = 10;
    [SerializeField] private float  _cooldown        = 0;

    public string Info { get => _info; set => _info = value; }

    private bool _active;
    public bool NotActive { get => _active; set => _active = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    private void Update()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
    }

    public void Use(GameObject player)
    {
        if (_cooldown > 0)
        {
            return;
        }
        _used?.Invoke();
        _gun = player.GetComponent<Gun>();
        _gun._coffeeBuf = true;
        Invoke("CoffeeBuffHandler", coffeeBuffTime);
        _cooldown = _defaultCooldown;
    }

    private void CoffeeBuffHandler()
    {
        _gun._coffeeBuf = false;
    }
}