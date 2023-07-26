using System;
using UnityEngine;

public interface Interactable
{
    public string Info { get; set; }

    public string NotActiveInfo { get; set; }

    public bool NotActive { get; set; }

    public Action Used { get; set; }

    public void UpdateInfo();

    public abstract void Use(GameObject player);
}
