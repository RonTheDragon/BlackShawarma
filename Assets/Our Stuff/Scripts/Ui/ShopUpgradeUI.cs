using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ShopUpgradeUI : MonoBehaviour
{

    [SerializeField] private TMP_Text  _productName,    _productDescription, _price, _level;
    [SerializeField] private Image     _productPicture, _levelBar;
                     private int       _currentLevel,   _maxLevel;
                     private List<int> _costs;
    [SerializeField] private Button    _button;     
    private GameManager   _gm            => GameManager.Instance;
    private Action<int>   _onBuy;
    private SOUpgrade _theUpgradeSO;
    public void SetupUpgrade(SOUpgrade theUpgradeSO, Action<int> onClickMethod)
    {
        _theUpgradeSO                   = theUpgradeSO;
        _productName.text              = _theUpgradeSO.UpgradeName;
        _productDescription.text       = _theUpgradeSO.UpgradeDescription;
        _costs                         = _theUpgradeSO.Costs;
        _maxLevel                      = _theUpgradeSO.Costs.Count;
        _price.text                    = $"{_costs[0]}¤";
        _productPicture.sprite         = _theUpgradeSO.UpgradeIcon;
        _level.text                    = "Buy";
        _levelBar.fillAmount           = 0;
        _onBuy                         = onClickMethod;
        _button.onClick.AddListener(() => TryToBuy());
    }

    public void TryToBuy()
    {
        if (_gm.GetMoney() >= _costs[_currentLevel])
        {
            Debug.Log("Baught Upgrade");
            _gm.OnTryToBuy(true);
            _gm.AddMoney(-_costs[_currentLevel]); // remove money
            _onBuy?.Invoke(_currentLevel);        // Upgrade

            _currentLevel++;
            _level.text          = $"{_currentLevel}/{_maxLevel}";          // level text, 1/3
            _levelBar.fillAmount = (float)_currentLevel / (float)_maxLevel; // levelbar set fill amount

            if (_maxLevel > _currentLevel) // if more upgrades exist
            {
                _price.text = $"{_costs[_currentLevel]}¤";
            }

            else // if fully upgraded
            {
                _button.interactable = false;
                _price.text          = $"MAX";
            }
        }
        else
        {
            _gm.OnTryToBuy(false);
            Debug.Log("Not Enough _money");
        }
    }

    public void RemoveLevel()
    {
        if (_currentLevel > 0)
        {
            _currentLevel--;
            _button.interactable = true;
            _price.text          = $"{_costs[_currentLevel]}¤";
            _level.text          = $"{_currentLevel}/{_maxLevel}";          // level text, 1/3
            _levelBar.fillAmount = (float)_currentLevel / (float)_maxLevel; // levelbar set fill amount
        }
        else
        {
            Debug.Log("used item after you were supposed to run out");
        }
    }

    public Action<int> GetAction()
    {
        return _onBuy;
    }

    public SOUpgrade GetSOUpgrade()
    {
        return _theUpgradeSO;
    }
}
