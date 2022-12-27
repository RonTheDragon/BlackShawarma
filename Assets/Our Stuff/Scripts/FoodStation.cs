using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStation : MonoBehaviour , Interactable
{
    [SerializeField]
    private string _info;
    public string Info { get => _info; set => _info = value; }

    [SerializeField] GameObject Panel;

    public void Use(GameObject player)
    {
        Gun g = player.GetComponent<Gun>();

        Panel.SetActive(!Panel.activeSelf);
        if (g != null)
        {
            g.UsingStation();
        }

        BuildOrder b = player.GetComponent<BuildOrder>();
        b.hasSupplies = false;
    }
}
