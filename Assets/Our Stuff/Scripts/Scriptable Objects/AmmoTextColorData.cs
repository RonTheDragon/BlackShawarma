using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Text Color Data", menuName = "Our Stuff/Ammo Text Color Data")]

public class AmmoTextColorData : ScriptableObject
{
    public AmmoAndColorTextPair[] ammoCounterData;
}

[System.Serializable]
public class AmmoAndColorTextPair
{
    public int   ammoType;
    public Color textColor;
}
