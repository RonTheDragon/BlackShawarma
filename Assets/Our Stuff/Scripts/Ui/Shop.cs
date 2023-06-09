using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<SOUpgrade> _upgrades;
    [SerializeField] private GameObject      _upgradePrefab;
    [SerializeField] private Transform       _upgradesContent;
    [SerializeField] private Camera          _shopCamera;
    [SerializeField] private Transform       _selectedProductSpot;
    [SerializeField] private Transform       _pointingAt;
    [SerializeField] private Rig             _homelessRig;
    [SerializeField] private float           _homelessWeightChangeSpeed = 10f;

    [SerializeField] private List<SOAmmoType> _ammoTypes = new List<SOAmmoType>();

    private List<ShopUpgradeUI> _upgradesUI = new List<ShopUpgradeUI>();

    private GameManager _gm => GameManager.Instance;
    private ShopProduct _productHit;
    private ShopProduct _productSelected;
    private float       _targetWeight = 0;

    private void Start()
    {
        _gm.TheShop = this;
        BuildShop();
        ResetAmmoTypes();
        _gm.TakeDamage += () => { if (_gm.MaxTzadokHp == 4) { RemoveUpgradeLevel(SOUpgrade.Upgrade.Armor); _gm.MaxTzadokHp = 3; } };
    }

    private void Update()
    {
        ShopRaycast();
    }

    private void ShopRaycast()
    {
        if (!_shopCamera.isActiveAndEnabled) return;

        RaycastHit hit;
        Ray ray = _shopCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            // Object hit by the raycast
            hit.collider.gameObject.TryGetComponent<ShopProduct>(out ShopProduct _newProductHit);
            if (_newProductHit == null) { _productHit?.OnHoverExit(); return; }

            if (_newProductHit != _productHit) { _productHit?.OnHoverExit(); _productHit = _newProductHit; }

            // Check for click input
            if (_productHit == null) { return; }

            if (Input.GetMouseButtonDown(0))
            {
                // Left mouse button clicked
                _productHit.OnClick();
            }

            // Check for hover enter
            if (!_productHit.GetIsHovered())
            {
                _productHit.OnHoverEnter();
            }
        }
        else
        {
            // No object hit within the maximum distance
            // Perform hover exit on the previously hovered object, if any
            _productHit?.OnHoverExit();
        }
        HomelessWeightRig();
    }

    private void HomelessWeightRig()
    {
        _homelessRig.weight = Mathf.Lerp(_homelessRig.weight,_targetWeight,Time.deltaTime * _homelessWeightChangeSpeed);
    }

    private void BuildShop()
    {
        foreach (SOUpgrade upgrade in _upgrades) //build shop
        {
            ShopUpgradeUI upgradeUI = Instantiate(_upgradePrefab, _upgradesContent.position, Quaternion.identity,
                _upgradesContent).GetComponent<ShopUpgradeUI>();

            upgradeUI.SetupUpgrade(upgrade, enumToAction(upgrade.UpgradeType));
            _upgradesUI.Add(upgradeUI);
            
        }
        HideAllUpgradesUI();
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
            case SOUpgrade.Upgrade.MoreFalafel:  return UpgradeFalafel;
            case SOUpgrade.Upgrade.MoreFries:    return UpgradeFries;
            case SOUpgrade.Upgrade.MoreEggplant: return UpgradeEggplant;
            case SOUpgrade.Upgrade.Armor:        return Armor;
            case SOUpgrade.Upgrade.Chili:        return Chili;
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
    private void Armor(int level)
    {
        Debug.Log($"Upgraded Armor Level {level}");
        _gm.MaxTzadokHp += 1;
    }

    private void Chili(int level)
    {
        Debug.Log($"Upgraded Chili Level {level}");

    }


    public void RemoveUpgradeLevel(SOUpgrade.Upgrade upgradeType)
    {
        foreach (Transform t in _upgradesContent)
        {
            ShopUpgradeUI ui = t.GetComponent<ShopUpgradeUI>();
            
            if (ui.GetAction() == enumToAction(upgradeType))
            {
                ui.RemoveLevel(); return;
            }
        }
    }

    public void ShowUpgrade(ShopProduct sp)
    {
        ShopUpgradeUI su = _upgradesUI.Find(x => x.GetSOUpgrade() == sp.TheUpgradeSO);
        if (su == null) return;

        if (su.gameObject.activeSelf) 
        {
            HideAllUpgradesUI();
            sp.SetDestinationBack();
            return; 
        }

        HideAllUpgradesUI();

        su.gameObject.SetActive(true);

        if (_productSelected != null)
        {
            _productSelected.SetDestinationBack();
        }
        _productSelected = sp;
        sp.SetDestination(_selectedProductSpot.position);
    }

    public void SetHomelessPointing(Vector3 pos)
    {
        _pointingAt.position = pos;
        _targetWeight = 1;
    }

    public void SetHomelessStopPointing()
    {
        _targetWeight = 0;
    }

    public void HideAllUpgradesUI()
    {
        foreach(ShopUpgradeUI i in _upgradesUI)
        {
            i.gameObject.SetActive(false);
        }
    }
}
