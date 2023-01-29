using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SideOrderUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _order;
                   //private string   _savedOrder;
                             bool     _tooAngry;
                             bool     _started;
    [SerializeField] private Image    _guyImage;
    [SerializeField] private Image    _angerBar;
    private EnemyAI _enemy;

    [SerializeField] private List<GameObject> _fillers;
    

    public void SetUp(EnemyAI enemy)
    {
        //string ordertxt = string.Empty;

        //for (int i = 0; i < enemy.Order.Count; i++)
        //{
        //    if (i == enemy.Order.Count - 1)          
        //        ordertxt += $"{enemy.Order[i]}.";
        //    else
        //         ordertxt += $"{enemy.Order[i]}, ";
        //}

        //_savedOrder = ordertxt;
        _guyImage.sprite = enemy.Picture;
        _enemy=enemy; 
        enemy.OnRageAmountChange += ChangeRageBar;
    }

    private void ChangeRageBar(float f,bool tooAngry)
    {
        _angerBar.fillAmount = f;
        
        if (_tooAngry != tooAngry || !_started)
        {
            _started = true;
            _tooAngry = tooAngry;

            // _order.text = tooAngry ? "???" : _savedOrder;
            if (tooAngry)
            {
                _order.text = "???";
                foreach(GameObject go in _fillers)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                _order.text = "";
                foreach(BuildOrder.Fillers filler in _enemy.Order)
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
