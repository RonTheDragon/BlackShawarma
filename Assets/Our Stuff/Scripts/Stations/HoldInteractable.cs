using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HoldInteractable : Interactable
{
    public bool HoldToUse { get; set; }
    public float UseDuration { get; set; }
}
