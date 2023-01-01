using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    Gun gun => player.GetComponent<Gun>();
    [SerializeField] TMP_Text Info;
    [SerializeField] TMP_Text Ammo;
    [SerializeField] TMP_Text MoneyText;
    // Start is called before the first frame update
    void Start()
    {
        gun.infoUpdate += UpdateInfo;
        gun.OnSwitchWeapon += SwitchAmmoType;
        gun.OnPitaAim += SwitchToPita;       
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoney();

    }

    void UpdateInfo(string info)
    {
        Info.text = info;
    }

    void SwitchAmmoType(AmmoType a)
    {
        Ammo.color = a.AmmoColor;
        Ammo.text = $"{a.AmmoTag}\nAmmo:\n{a.CurrentAmmo}/{a.MaxAmmo}";
    }

    void SwitchToPita(List<BuildOrder.Fillers> pita)
    {
        Ammo.color = Color.white;
        string txt = "Pita with:";
        foreach(BuildOrder.Fillers f in pita)
        {
            txt += $"\n{f}";
        }
        Ammo.text = txt;
    }
    void UpdateMoney()
    {
         MoneyText.text = "joobot ="+ GameManager.instance.Money.ToString() ;
    }
}
