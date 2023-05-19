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
    [SerializeField] private Button   _button;
    private EnemyAI _enemy;
    private GameManager _gm;

    public List<GameObject> Fillers= new List<GameObject>();

    

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
        _gm = GameManager.Instance;
        _button.gameObject.SetActive(false);
    }

    public void ButtonClick()
    {
        _gm.MaximizeOrder(this);
    }

    private void OnDestroy()
    {
        _gm.UnMaximizeOrder(this);
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
                _button.gameObject.SetActive(false);
                _pitaIcon.enabled = false;
                _wantedFoodIcon.enabled = true;
                foreach (GameObject go in Fillers)
                {
                    go.SetActive(false);
                }
                if (_showSecondsLeft <= 0) 
                {
                    gameObject.SetActive(false);
                }
                _gm.UnMaximizeOrder(this);
            }
            else
            {
                gameObject.SetActive(true);
                _guyImage.sprite = _guyHappyImage;
                _angerBar.color = Color.green;
                _button.gameObject.SetActive(true);
                _pitaIcon.enabled = true;
                _wantedFoodIcon.enabled = false;
                foreach (BuildOrder.Fillers filler in _enemy.Order)
                {
                    switch (filler)
                    {
                        case BuildOrder.Fillers.Humus:
                            Fillers[0].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Pickles:
                            Fillers[1].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Cabbage:
                            Fillers[2].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Onions:
                            Fillers[3].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Salad:
                            Fillers[4].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Spicy:
                            Fillers[5].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Amba:
                            Fillers[6].SetActive(true);
                            break;
                        case BuildOrder.Fillers.Thina:
                            Fillers[7].SetActive(true);
                            break;
                    }
                }
            }
        }
    }
}
