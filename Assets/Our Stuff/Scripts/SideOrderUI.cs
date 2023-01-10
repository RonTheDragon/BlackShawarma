using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SideOrderUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _order;
                     private string   _savedOrder;
                             bool     _tooAngry;
                             bool     _started;
    [SerializeField] private Image    _guyImage;
    [SerializeField] private Image    _angerBar;
    

    public void SetUp(EnemyAI enemy)
    {
        string ordertxt = string.Empty;

        for (int i = 0; i < enemy.Order.Count; i++)
        {
            if (i == enemy.Order.Count - 1)          
                ordertxt += $"{enemy.Order[i]}.";
            else
                 ordertxt += $"{enemy.Order[i]}, ";
        }

        _savedOrder = ordertxt;
        _guyImage.sprite = enemy.Picture;
        enemy.OnRageAmountChange += ChangeRageBar;
    }

    private void ChangeRageBar(float f,bool tooAngry)
    {
        _angerBar.fillAmount = f;
        
        if (_tooAngry != tooAngry || !_started)
        {
            _started = true;
            _tooAngry = tooAngry;

            _order.text = tooAngry ? "???" : _savedOrder;
        }
    }
}
