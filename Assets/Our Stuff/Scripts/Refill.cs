using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour, Interactable
{
    [SerializeField] SOAmmoType _type;
    [SerializeField] private string _info;
    public string Info { get => _info; set => _info = value; }

    private Camera PlayerCamera => Camera.main;

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
