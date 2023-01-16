using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] private List<SOUpgrade> _upgrades;
    [SerializeField] private GameObject _upgradePrefab;
    [SerializeField] private Transform _upgradesContent;

    [SerializeField] private List<SOAmmoType> _ammoTypes = new List<SOAmmoType>(); 

    // Start is called before the first frame update
    private void Start()
    {
        BuildShop();
        ResetAmmoTypes();
    }

    private void BuildShop()
    {
        foreach (SOUpgrade upgrade in _upgrades) //build shop
        {
            ShopUpgradeUI upgradeUI = Instantiate(_upgradePrefab, _upgradesContent.position, Quaternion.identity,
                _upgradesContent).GetComponent<ShopUpgradeUI>();

            upgradeUI.SetupUpgrade(upgrade.UpgradeName, upgrade.UpgradeDescription,
                upgrade.Costs, upgrade.UpgradeIcon, enumToAction(upgrade.UpgradeType));
        }
    }

    private void ResetAmmoTypes()
    {
        foreach (SOAmmoType ammo in _ammoTypes)
        {
            ammo.MaxAmmo = 10;
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

        switch (level)
        {
            case 0: _ammoTypes[0].MaxAmmo = 12; break;
            case 1: _ammoTypes[0].MaxAmmo = 15; break;
            case 2: _ammoTypes[0].MaxAmmo = 20; break;
        }
    }
    private void UpgradeFries(int level)
    {
        Debug.Log($"Upgraded Fries Level {level}");

        switch (level)
        {
            case 0: _ammoTypes[1].MaxAmmo = 12; break;
            case 1: _ammoTypes[1].MaxAmmo = 15; break;
            case 2: _ammoTypes[1].MaxAmmo = 20; break;
        }
    }
    private void UpgradeEggplant(int level)
    {
        Debug.Log($"Upgraded Eggplant Level {level}");

        switch (level)
        {
            case 0: _ammoTypes[2].MaxAmmo = 12; break;
            case 1: _ammoTypes[2].MaxAmmo = 15; break;
            case 2: _ammoTypes[2].MaxAmmo = 20; break;
        }
    }

}
