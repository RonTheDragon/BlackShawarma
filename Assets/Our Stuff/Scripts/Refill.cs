using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Refill : MonoBehaviour, Interactable
{
    [SerializeField] SOAmmoType _type;
    [SerializeField] private string _info;
    public string Info { get => _info; set => _info = value; }

    private bool _active;
    public bool NotActive { get => _active; set => _active = value; }
    private Camera PlayerCamera => Camera.main;

    private Action _used;
    public Action Used { get => _used; set => _used=value; }

    [SerializeField] private GameObject _ui;

    private void Update()
    {
        if (_type.CurrentAmmo == 0)
        {
           if (_ui.activeSelf==false) _ui.SetActive(true);
           _ui.transform.LookAt(PlayerCamera.transform.position);
        }
        else if (_ui.activeSelf == true)
        {
           _ui.SetActive(false);
        }
    }

    public void Use(GameObject player)
    {
        _used?.Invoke();

        Gun g = player.GetComponent<Gun>();

        foreach (SOAmmoType i in g.AmmoTypes)
        {
            if(i.FoodType == _type.FoodType)
            {
                i.CurrentAmmo = i.MaxAmmo;
                break;
            }
        }
    }

}
