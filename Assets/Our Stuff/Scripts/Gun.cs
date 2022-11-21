using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //Serializefield 
    [Tooltip("shooting cooldown")]
    [SerializeField] float CoolDown = 0.5f;
    [SerializeField] Transform barrel;
    [SerializeField] Transform cam;
    //Private 
    float _cd;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position,cam.forward,out hit,Mathf.Infinity))
        {
            barrel.LookAt(hit.point);
        }
        else
        {
            barrel.LookAt(cam.position+cam.forward*200);
        }
        if (Input.GetMouseButton(0) && _cd <= 0)
        {
            GameObject bullet = ObjectPooler.Instance.SpawnFromPool("bullet", barrel.position, barrel.rotation);
            _cd = CoolDown;
        }
        if (_cd > 0)
        {
            _cd -= Time.deltaTime;

        }
    }

}
