using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    Gun gun => player.GetComponent<Gun>();
    BuildOrder BO => player.GetComponent<BuildOrder>();

    GameManager GM;
    LevelTimer LT;

    [SerializeField] List<Image> Fillers;

    [SerializeField] TMP_Text Info;
    [SerializeField] TMP_Text Ammo;
    [SerializeField] TMP_Text MoneyText;
    [SerializeField] TMP_Text Timer;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        LT = GM.GetComponent<LevelTimer>();

        GM.UpdateMoney += UpdateMoney;
        gun.infoUpdate += UpdateInfo;
        gun.OnSwitchWeapon += SwitchAmmoType;
        gun.OnPitaAim += SwitchToPita;
        BO.OnUseIngridients += UpdateIngridients;
        LT.OnUpdateTimer += UpdateTimer;

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
         MoneyText.text = "Joobot = "+ GM.GetMoney().ToString() + "¤";
    }

    void UpdateIngridients(List<Ingredient> ingredients)
    {
        for (int i = 0; i < Fillers.Count; i++)
        {
            Fillers[i].fillAmount = (float)ingredients[i].CurrentAmount / (float)ingredients[i].MaxAmount;
        }
    }

    void UpdateTimer(int seconds, int minutes)
    {
        string S, M;

        if (seconds < 10)  S = $"0{seconds}";
        else S = $"{seconds}";

        if (minutes < 10) M = $"0{minutes}";
        else M = $"{minutes}";

        Timer.text = $"Time Left: {M}:{S}";
    }
}
