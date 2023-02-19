using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStation : MonoBehaviour , Interactable
{
    [SerializeField]
    private string _info;
    public string Info { get => _info; set => _info = value; }

    [SerializeField] GameObject Panel;

    private Gun _gun;

    public void Use(GameObject player)
    {
        _gun = player.GetComponent<Gun>();
        BuildOrder b = player.GetComponent<BuildOrder>();
        Panel.SetActive(!Panel.activeSelf);

        if (_gun != null)
        {
            if (!_gun.UsingUI)
                _gun.StartUsingStation();
            else
            {
                _gun.StopUsingStation();
                _gun = null;
            }
        }

        if (b.HasSupplies)
        {
            b.FillAll();
            b.HasSupplies = false;
        }
    }
}
