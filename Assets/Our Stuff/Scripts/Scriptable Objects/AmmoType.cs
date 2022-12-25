using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Type", menuName = "Food")]

public class AmmoType : ScriptableObject
{
    public string AmmoTag;
    public Adible.Food FoodType;
    public Material TrajectoryMaterial;
    public Color AmmoColor;
    [HideInInspector]public int CurrentAmmo;
    public int MaxAmmo;
}

