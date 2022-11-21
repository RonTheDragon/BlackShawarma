using Cinemachine;
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
    [SerializeField] CinemachineFreeLook cinemachine;
    [ReadOnly][SerializeField] bool isAiming;
    [SerializeField] int fov_aim =40;
    [SerializeField] int fovnot_aim=70;
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
        Aim();
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
    void Aim()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }
        if (isAiming)
        {
            cinemachine.m_Lens.FieldOfView = fov_aim;

        }
        else if(!isAiming)
        {
           cinemachine.m_Lens.FieldOfView = fovnot_aim;
        }
    }

}
