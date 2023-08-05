using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Shooting")]
    [field: SerializeField] public EventReference shooting { get; private set; }
    [field: Header("ShootingPita")]
    [field: SerializeField] public EventReference shootingPita { get; private set; }
    [field: Header("Filler")]
    [field: SerializeField] public EventReference Filler { get; private set; }
    [field: Header("Hit")]
    [field: SerializeField] public EventReference hit { get; private set; }
    [field: Header("Pick")]
    [field: SerializeField] public EventReference Pick { get; private set; }
    [field: Header("Chipser")]
    [field: SerializeField] public EventReference Chipser { get; private set; }
  
    [field: SerializeField] public EventReference ChipserUse { get; private set; }

    [field: Header("OutOfAmmo")]
    [field: SerializeField] public EventReference OutOfAmmo { get; private set; }
    [field: Header("Counter")]
    [field: SerializeField] public EventReference Counter { get; private set; }
    [field: Header("Ars")]
    [field: SerializeField] public EventReference [] ArsTalk { get; private set; }
    [field: Header("Arsit")]
    [field: SerializeField] public EventReference[] ArsitTalk { get; private set; }
    [field: Header("Hipster")]
    [field: SerializeField] public EventReference[] HipsterTalk { get; private set; }
    [field: Header("OldGuy")]
    [field: SerializeField] public EventReference[] OldGuyTalk { get; private set; }
    [field: Header("Cop")]
    [field: SerializeField] public EventReference[] CopTalk { get; private set; }
    [field: Header("Soldier")]
    [field: SerializeField] public EventReference[] SoldierTalk { get; private set; }
    [field: Header("Mobster")]
    [field: SerializeField] public EventReference[] MobsterTalk { get; private set; }
    [field: Header("Tutorial")]
    [field: SerializeField] public EventReference[] Tutorial { get; private set; }
    [field: Header("Music")]
    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    //[field: Header("Player SFX")]
    //[field: SerializeField] public EventReference playerFootsteps { get; private set; }

    //[field: Header("Coin SFX")]
    //[field: SerializeField] public EventReference coinCollected { get; private set; }
    //[field: SerializeField] public EventReference coinIdle { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}