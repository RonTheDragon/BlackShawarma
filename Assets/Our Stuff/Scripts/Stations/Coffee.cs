using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coffee : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
    [SerializeField] public int coffeeBuffTime = 5;
    private Gun _gun;

    public string Info { get => _info; set => _info = value; }

    private bool _active;
    public bool NotActive { get => _active; set => _active = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }
    public void Use(GameObject player)
    {
        _used?.Invoke();
        _gun = player.GetComponent<Gun>();
        _gun._coffee = true;
        Invoke("CoffeeBuffHandler", coffeeBuffTime);

    }

    private void CoffeeBuffHandler()
    {
        _gun._coffee = false;
    }
}
