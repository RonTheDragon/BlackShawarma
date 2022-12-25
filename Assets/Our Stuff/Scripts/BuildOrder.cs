using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildOrder : MonoBehaviour
{
    [SerializeField] int MaxFillers;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddHumus()
    {
        AddFiller(Fillers.Humus);
    }
    public void AddPickles()
    {
        AddFiller(Fillers.Pickles);
    }
    public void AddCabbage()
    {
        AddFiller(Fillers.Cabbage);
    }
    public void AddOnions()
    {
        AddFiller(Fillers.Onions);
    }
    public void AddSalad()
    {
        AddFiller(Fillers.Salad);
    }
    public void AddSpicy()
    {
        AddFiller(Fillers.Spicy);
    }
    public void AddAmba()
    {
        AddFiller(Fillers.Amba);
    }
    public void AddThina()
    {
        AddFiller(Fillers.Thina);
    }

    public void AddFiller(Fillers filler)
    {
        if (Pita.Count == MaxFillers) return;
        if (Pita.Contains(filler)) return;

        Pita.Add(filler);
        ReadOutPita();
    }

    public void Trash()
    {
        Pita.Clear();
        ReadOutPita();
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
