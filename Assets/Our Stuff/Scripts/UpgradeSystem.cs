using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] private List<SOUpgrade> _upgrades;
    [SerializeField] private GameObject _upgradePrefab;
    [SerializeField] private Transform _upgradesContent;

    // Start is called before the first frame update
    private void Start()
    {
        foreach(SOUpgrade upgrade in _upgrades) //build shop
        {
            ShopUpgradeUI upgradeUI = Instantiate(_upgradePrefab, _upgradesContent.position,Quaternion.identity,
                _upgradesContent).GetComponent<ShopUpgradeUI>();

            upgradeUI.SetupUpgrade(upgrade.UpgradeName,upgrade.UpgradeDescription,
                upgrade.Costs,upgrade.UpgradeIcon, enumToAction(upgrade.UpgradeType));
        }
    }

    private Action<int> enumToAction(SOUpgrade.Upgrade upgrade)
    {
        switch (upgrade)
        {
            case SOUpgrade.Upgrade.MoreFalafel: return UpgradeFalafel;
            case SOUpgrade.Upgrade.MoreFries: return UpgradeFries;
            case SOUpgrade.Upgrade.MoreEggplant: return UpgradeEggplant;
        }
        return null;
    }

    private void UpgradeFalafel(int level)
    {
        Debug.Log($"Upgraded Falafel Level {level}");
    }
    private void UpgradeFries(int level)
    {
        Debug.Log($"Upgraded Fries Level {level}");
    }
    private void UpgradeEggplant(int level)
    {
        Debug.Log($"Upgraded Eggplant Level {level}");
    }

}
