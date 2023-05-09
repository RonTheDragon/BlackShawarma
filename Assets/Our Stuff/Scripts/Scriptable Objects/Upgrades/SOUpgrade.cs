using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class SOUpgrade : ScriptableObject
{
    public string UpgradeName;
    public string UpgradeDescription;
    public Sprite UpgradeIcon;
    public List<int> Costs = new List<int>();
    public Upgrade UpgradeType;
    public enum Upgrade : int
    {
        MoreFalafel,
        MoreFries,
        MoreEggplant,
        Armor,
        Coffee
    }
}