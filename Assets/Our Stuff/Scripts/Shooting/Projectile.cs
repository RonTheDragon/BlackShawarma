using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour,IpooledObject
{
    [Tooltip("the force of the bullet ")]
    [SerializeField] protected float bullet_speed =100f;
    protected Rigidbody body => GetComponent<Rigidbody>();
    private TrailRenderer _tr => GetComponentInChildren<TrailRenderer>();
    //private VisualEffect _vfx => GetComponentInChildren<VisualEffect>();
    public void OnObjectSpawn()
    {
        body.velocity = Vector3.zero;
        body.AddForce(transform.forward * bullet_speed,ForceMode.Impulse);
        _tr.Clear();
        //_vfx.SendEvent("OnPlay");
    }

    
}
