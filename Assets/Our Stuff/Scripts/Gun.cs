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
    [Header("Aiming")]
    [SerializeField] float AimingFOV =40;
    [SerializeField] float NotAimingFOV=70;
    [SerializeField] float FovChangingSpeed = 60;
    [ReadOnly][SerializeField] float CurrentFOV;
    [ReadOnly][SerializeField] bool isAiming;
    [Header("Ammo Switching")]
    [ReadOnly][SerializeField] string CurrentAmmo;
    [SerializeField] List<string> AmmoTypes;


    //Private 
    float _cd;
    int _currentAmmo;
    
    // Start is called before the first frame update
    void Start()
    {
        cinemachine.m_Lens.FieldOfView = NotAimingFOV;
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        Aim();
        AmmoSwitching();
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
            GameObject bullet = ObjectPooler.Instance.SpawnFromPool(CurrentAmmo, barrel.position, barrel.rotation);
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

        CurrentFOV = cinemachine.m_Lens.FieldOfView;
        if (isAiming)
        {
            if (CurrentFOV > AimingFOV)
            {
                cinemachine.m_Lens.FieldOfView -= FovChangingSpeed * Time.deltaTime;
            }
            else { cinemachine.m_Lens.FieldOfView = AimingFOV; }

        }
        else
        {
            if (CurrentFOV < NotAimingFOV)
            {
                cinemachine.m_Lens.FieldOfView += FovChangingSpeed * Time.deltaTime;
            }
            else { cinemachine.m_Lens.FieldOfView = NotAimingFOV; }
        }
    }

    void AmmoSwitching()
    {
        if (AmmoTypes.Count > 1)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                _currentAmmo++;
                if (_currentAmmo > AmmoTypes.Count-1)
                {
                    _currentAmmo = 0;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                _currentAmmo--;
                if (_currentAmmo < 0)
                {
                    _currentAmmo = AmmoTypes.Count-1;
                }
            }
            CurrentAmmo = AmmoTypes[_currentAmmo];
        }
    }

}
