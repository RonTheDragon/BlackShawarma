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
        Gun        g = player.GetComponent<Gun>();
        BuildOrder b = player.GetComponent<BuildOrder>();
        Panel.SetActive(!Panel.activeSelf);

        if (g != null)
        {
            if (!g.OnStation)
                g.StartUsingStation();
            else
                g.StopUsingStation();
        }

        if (b.HasSupplies)
        {
            b.FillAll();
            b.HasSupplies = false;
        }
    }
}
