using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public string Info { get; set; }

    public bool NotActive { get; set; }

    public Action Used { get; set; }

    public abstract void Use(GameObject player);
}
