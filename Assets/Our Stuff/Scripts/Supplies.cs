using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supplies : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
    public string Info { get => _info; set => _info = value; }

    public void Use(GameObject player)
    {
        BuildOrder b = player.GetComponent<BuildOrder>();

        if (b != null)
        {
            b.HasSupplies = true;
        }
    }
}
