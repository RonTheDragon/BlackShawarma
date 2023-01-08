using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Ingredient
{
    public Ingredient(BuildOrder.Fillers Type, int MaxAmount)
    {
        this.Type = Type;
        this.MaxAmount = MaxAmount;
        CurrentAmount = MaxAmount;
    }
    public BuildOrder.Fillers Type;
    public int MaxAmount;
    public int CurrentAmount;
}


public class BuildOrder : MonoBehaviour
{
    [SerializeField] int MaxFillers;

    public bool hasSupplies;

    public Action<List<Ingredient>> OnUseIngridients;

    public int supplies = 8;
    [SerializeField] List<Ingredient> ingredients = new List<Ingredient>();
    public enum Fillers : int
    {
        Humus,
        Pickles,
        Cabbage,
        Onions,
        Salad,
        Spicy,
        Amba,
        Thina
    }

    public List<Fillers> Pita = new List<Fillers>();

    void Start()
    {
        int count  = Enum.GetValues(typeof(BuildOrder.Fillers)).Length;
        MaxFillers = GameManager.instance.MaxFillers;
        for (int i = 0; i < count; i++)
        {
            Fillers f = (Fillers)i;
            ingredients.Add(new Ingredient(f,3));
        }
    }

    public void AddFiller(int fillerNumber)
    {
        Fillers filler = (Fillers)fillerNumber;
        if (Pita.Count == MaxFillers) return;
        if (Pita.Contains(filler)) return;
        Ingredient i = ingredients.Find(x => x.Type == filler);
        if (i.CurrentAmount <= 0) return;
        i.CurrentAmount--;
        OnUseIngridients?.Invoke(ingredients);
        Pita.Add(filler);
        ReadOutPita();
    }

    public void Trash()
    {
        Pita.Clear();
        ReadOutPita();
    }

    public void GetPita()
    {
        if (Pita.Count > 0)
        {
            List<Fillers> temporary =  new List<Fillers>(Pita);
            GetComponent<Gun>().SetPita(temporary);       
            Pita.Clear();
        }
    }

    public void FillAll()
    {
        foreach (Ingredient i in ingredients) i.CurrentAmount = i.MaxAmount;
        OnUseIngridients?.Invoke(ingredients);
    }

    void ReadOutPita()
    {
        string msg = string.Empty;
        foreach (Fillers f in Pita)
        {
            msg += $"{f} ";
        }
        Debug.Log(msg);
    }
}
