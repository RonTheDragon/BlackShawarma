using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour, Interactable
{
    [SerializeField] Edible.Food FillType;

    [SerializeField] private string _info;
    public string Info { get => _info; set => _info = value; }

    public void Use(GameObject player)
    {
        Gun g = player.GetComponent<Gun>();

        foreach (SOAmmoType i in g.AmmoTypes)
        {
            if(i.FoodType == FillType)
            {
                i.CurrentAmmo = i.MaxAmmo;
                break;
            }
        }
    }

}
