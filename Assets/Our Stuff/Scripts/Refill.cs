using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour, Interactable
{
    [SerializeField] int ammoToRefill;
    [SerializeField]
    private string _info;
    public string Info { get => _info; set => _info = value; }

    public void Use(Gun g)
    {
        g.CurrentAmmoType.CurrentAmmo = g.CurrentAmmoType.MaxAmmo;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
