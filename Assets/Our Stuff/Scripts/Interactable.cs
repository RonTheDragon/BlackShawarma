using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public string Info { get; set; }

    public abstract void Use(GameObject player);
}
