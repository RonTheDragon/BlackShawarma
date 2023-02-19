using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SideOrderUI : MonoBehaviour
{
    [SerializeField] private float _showForSeconds = 3;
    [ReadOnly][SerializeField] private float _showSecondsLeft;

    [SerializeField] private TMP_Text _number;
                   //private string   _savedOrder;
                             bool     _tooAngry;
                             bool     _started;
    [SerializeField] private Image    _guyImage;
    private Sprite    _guyHappyImage;
    private Sprite    _guyAngryImage;
    [SerializeField] private Image    _pitaIcon;
    [SerializeField] private Image    _wantedFoodIcon;
    [SerializeField] private Image    _panel;
    [SerializeField] private Image    _angerBar;
    private EnemyAI _enemy;

    [SerializeField] private List<GameObject> _fillers;

    

    public void SetUp(EnemyAI enemy)
    {
        _panel.sprite = enemy.SideOrderPanel;
        _guyHappyImage = enemy.HappyPicture;
        _guyAngryImage = enemy.AngryPicture;
        _guyImage.sprite = _guyAngryImage;
        _number.text = $"{enemy.CustomerNumber}";
        _wantedFoodIcon.sprite = enemy.RequestedFood;

        _enemy =enemy; 
        enemy.OnRageAmountChange += ChangeRageBar;
        enemy.OnBeingShot += ShowRageBar;
    }

    private void ShowRageBar()
    {
        _showSecondsLeft = _showForSeconds;
        gameObject.SetActive(true);
    }

    private void ChangeRageBar(float f,bool tooAngry)
    {
        _angerBar.fillAmount = f;

        if (_showSecondsLeft > 0) { _showSecondsLeft -= Time.deltaTime; }
        else if (_tooAngry) { gameObject.SetActive(false); }

        if (_tooAngry != tooAngry || !_started)
        {
            _started = true;
            _tooAngry = tooAngry;

            // _order.text = tooAngry ? "???" : _savedOrder;
            if (tooAngry)
            {
                _guyImage.sprite = _guyAngryImage;
                _angerBar.color = Color.red;
                _pitaIcon.enabled = false;
                _wantedFoodIcon.enabled = true;
                foreach (GameObject go in _fillers)
                {
                    go.SetActive(false);
                }
                if (_showSecondsLeft <= 0) 
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(true);
                _guyImage.sprite = _guyHappyImage;
                _angerBar.color = Color.green;
                _pitaIcon.enabled = true;
                _wantedFoodIcon.enabled = false;
                foreach (BuildOrder.Fillers filler in _enemy.Order)
                {
                    switch (filler)
                    {
                        case BuildOrder.Fillers.Humus:
                            _fillers[0].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Pickles:
                            _fillers[1].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Cabbage:
                            _fillers[2].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Onions:
                            _fillers[3].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Salad:
                            _fillers[4].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Spicy:
                            _fillers[5].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Amba:
                            _fillers[6].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Thina:
                            _fillers[7].SetActive(true);
                            break;
                    }
                }
            }
        }
    }
}
