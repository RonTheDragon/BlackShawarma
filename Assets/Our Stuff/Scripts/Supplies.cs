using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Supplies : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
    public string Info { get => _info; set => _info = value; }

    private bool _active;
    public bool NotActive { get => _active; set => _active = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }
    public void Use(GameObject player)
    {
        _used.Invoke();

        BuildOrder b = player.GetComponent<BuildOrder>();

        if (b != null)
        {
            b.HasSupplies = true;
        }
    }
}
