using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour,IpooledObject
{
    [Tooltip("the force of the bullet ")]
    [SerializeField] protected float bullet_speed =100f;
    protected Rigidbody body => GetComponent<Rigidbody>();
    public void OnObjectSpawn()
    {
        body.velocity = Vector3.zero;
        body.AddForce(transform.forward * bullet_speed,ForceMode.Impulse);
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
