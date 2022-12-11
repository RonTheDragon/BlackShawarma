using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //Serializefield 
    [Tooltip("shooting cooldown")]
    [SerializeField] float CoolDown = 0.5f;

    [Header("Aiming")]
              [Tooltip("Field of view while aiming")]
              [SerializeField] float AimingFOV        = 40;
              [Tooltip("Field of view")]
              [SerializeField] float NotAimingFOV     = 70;
              [Tooltip("The seed of the transition from normal to aiming field of view")]
              [SerializeField] float FovChangingSpeed = 60;
              [Tooltip("The current field of view")]
    [ReadOnly][SerializeField] float CurrentFOV;
              [Tooltip("Is the character aiming or not")]
    [ReadOnly][SerializeField] bool  isAiming;

    [Header("Ammo Switching")]
    [Tooltip("The current amount of ammo")]
    [ReadOnly][SerializeField] string       CurrentAmmo;
              [Tooltip("The types of ammo")]
              [SerializeField] List<string> AmmoTypes;

    [Header("Refefrences")]
    [Tooltip("Reference to the point where projectiles spawn")]
    [SerializeField] Transform           barrel;
    [Tooltip("Camera reference")]
    [SerializeField] Transform           cam;
    [Tooltip("Reference to the cinemachine")]
    public CinemachineFreeLook cinemachine;

    ThirdPersonMovement tpm => GetComponent<ThirdPersonMovement>();

    [SerializeField] TMP_Text Info;
    //Event
    Action _loop;

    public Action<Gun> OnUse;

    //Private 
    float _cd;
    int   _currentAmmo;

    public bool OnStation;
    
    // Start is called before the first frame update
    void Start()
    {
        _loop += Shoot;
        _loop += Aim;
        _loop += AmmoSwitching;
        cinemachine.m_Lens.FieldOfView = NotAimingFOV;
    }

    // Update is called once per frame
    void Update()
    {
        _loop?.Invoke();   
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position,cam.forward,out hit,Mathf.Infinity))
        {
            barrel.LookAt(hit.point);
            if (hit.distance < 6)
            {
                Interactable interact = hit.transform.GetComponent<Interactable>();
                if (interact != null)
                {
                    Info.text = interact.Info;
                    OnUse = interact.Use;
                }
                else
                {                   
                    StoppedHoveringStation();
                }
            }
            else StoppedHoveringStation();
        }
        else
        {
            StoppedHoveringStation();
            barrel.LookAt(cam.position+cam.forward*200);
        }
        if (Input.GetMouseButton(0) && _cd <= 0 && !OnStation)
        {
            GameObject bullet = ObjectPooler.Instance.SpawnFromPool(CurrentAmmo, barrel.position, barrel.rotation);
            _cd = CoolDown;
        }
        if (_cd > 0)
        {
            _cd -= Time.deltaTime;

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnUse?.Invoke(this);
        }
        if (Input.GetKeyDown(KeyCode.Escape)&& OnStation)
        {
            OnUse?.Invoke(this);
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
    void StoppedHoveringStation()
    {
        Info.text = string.Empty;
        if (OnStation) { OnUse?.Invoke(this); }
        else
        {
            OnUse = null;
        }
    }

    public void UsingStation()
    {
            OnStation = !OnStation;
            cinemachine.enabled = !cinemachine.enabled;
            tpm.enabled = !tpm.enabled;
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;      
    }

}
