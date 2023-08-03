using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour
{
     private FMODEvents _fMODEvents;
    [SerializeField] private enemyTypes _currentEnemy;
    private StudioEventEmitter _enterEmitter;
    private StudioEventEmitter _angryEmitter;
    private StudioEventEmitter _exitEmitter;




    void Start()
    {
        _fMODEvents = FMODEvents.instance;
        _enterEmitter = gameObject.AddComponent<StudioEventEmitter>();
        _angryEmitter = gameObject.AddComponent<StudioEventEmitter>();
        _exitEmitter = gameObject.AddComponent<StudioEventEmitter>();
        switch (_currentEnemy)
        {
            case enemyTypes.ars:
                _enterEmitter.EventReference = _fMODEvents.ArsTalk[0];
                _angryEmitter.EventReference = _fMODEvents.ArsTalk[1];
                _exitEmitter.EventReference = _fMODEvents.ArsTalk[2];

                break;
            case enemyTypes.arsit:
                _enterEmitter.EventReference = _fMODEvents.ArsitTalk[0];
                _angryEmitter.EventReference = _fMODEvents.ArsitTalk[1];
                _exitEmitter.EventReference = _fMODEvents.ArsitTalk[2];

                break;
            case enemyTypes.hipster:
                _enterEmitter.EventReference = _fMODEvents.HipsterTalk[0];
                _angryEmitter.EventReference = _fMODEvents.HipsterTalk[1];
                _exitEmitter.EventReference = _fMODEvents.HipsterTalk[2];

                break;
            case enemyTypes.oldman:
                _enterEmitter.EventReference = _fMODEvents.OldGuyTalk[0];
                _angryEmitter.EventReference = _fMODEvents.OldGuyTalk[1];
                _exitEmitter.EventReference = _fMODEvents.OldGuyTalk[2];

                break;
            case enemyTypes.cop:
                _enterEmitter.EventReference = _fMODEvents.CopTalk[0];
                _angryEmitter.EventReference = _fMODEvents.CopTalk[1];
                _exitEmitter.EventReference = _fMODEvents.CopTalk[2];

                break;
            case enemyTypes.soldier:
                _enterEmitter.EventReference = _fMODEvents.SoldierTalk[0];
                _angryEmitter.EventReference = _fMODEvents.SoldierTalk[1];
                _exitEmitter.EventReference = _fMODEvents.SoldierTalk[2];

                break;
            case enemyTypes.mobster:
                _enterEmitter.EventReference = _fMODEvents.MobsterTalk[0];
                _angryEmitter.EventReference = _fMODEvents.MobsterTalk[1];
                _exitEmitter.EventReference = _fMODEvents.MobsterTalk[2];

                break;
            default:
                break;
        }
    }
    enum enemyTypes
    {
        ars,
        arsit,
        hipster,
        oldman,
        cop,
        soldier,
        mobster

    }
    public void PlayEnterSound()
    {
        _enterEmitter.Play();
    }
    public void PlayAngrySound()
    {
        _angryEmitter.Play();
    }
    public void PlayExitSound()
    {
        _exitEmitter.Play();
    }
  
}
