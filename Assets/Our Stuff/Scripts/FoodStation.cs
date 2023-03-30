using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStation : MonoBehaviour , Interactable
{
    [SerializeField]
    private string _info;
    public string Info { get => _info; set => _info = value; }

    private bool _active;
    public bool NotActive { get => _active; set => _active = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    [SerializeField] GameObject Panel;

    private Gun _gun;

    public void Use(GameObject player)
    {
        _used?.Invoke();
        _gun = player.GetComponent<Gun>();
        BuildOrder b = player.GetComponent<BuildOrder>();
        Panel.SetActive(!Panel.activeSelf);

        if (_gun != null)
        {
            if (!_gun.UsingUI)
                _gun.StartUsingStation();
            else
            {
                _gun.StopUsingStation();
                _gun = null;
            }
        }

        if (b.HasSupplies)
        {
            b.FillAll();
            b.HasSupplies = false;
        }
    }
}
