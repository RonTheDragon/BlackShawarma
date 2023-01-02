using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //Serializefield 
    [Tooltip("shooting cooldown")]
    [SerializeField] float CoolDown = 0.5f;

    [Header("Aiming")]
    [Tooltip("Field of view while aiming")]
    [SerializeField] float AimingFOV = 3;
    [Tooltip("Field of view")]
    [SerializeField] float NotAimingFOV = 0;
    [Tooltip("The speed of transition from normal to aiming field of view")]
    [SerializeField] float FovChangingSpeed = 12;
    [Tooltip("The current field of view")]
    [ReadOnly][SerializeField] float CurrentFOV;
    [Tooltip("Is the character aiming or not")]
    [ReadOnly][SerializeField] bool isAiming;

    [Header("Ammo Switching")]
    [Tooltip("The current amount of ammo")]
    [ReadOnly]public AmmoType CurrentAmmoType;
    [Tooltip("The types of ammo")]
    public List<AmmoType> AmmoTypes;

    [Header("Refefrences")]
    [Tooltip("Reference to the point where projectiles spawn")]
    [SerializeField] Transform barrel;
    [Tooltip("Camera reference")]
    [SerializeField] Transform cam;
    [Tooltip("Reference to the cinemachine")]
    public CinemachineFreeLook cinemachine;
    CinemachineCameraOffset offset => cinemachine.GetComponent<CinemachineCameraOffset>();
    ThirdPersonMovement tpm => GetComponent<ThirdPersonMovement>();

    [Header("Projection")]
    [SerializeField]
    [Range(10, 100)]
    private int linepoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float timeBetweenPoints = 0.1f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LayerMask layer;

    //[SerializeField] TMP_Text Info;
    //Event
    Action _loop;

    public Action<String> infoUpdate;

    public Action<GameObject> OnUse;

    public Action<AmmoType> OnSwitchWeapon;

    public Action<List<BuildOrder.Fillers>> OnPitaAim;

    List<BuildOrder.Fillers> _currentPita = new List<BuildOrder.Fillers>();

    [SerializeField] Material PitaTrajectoryMaterial;

    //Private 
    float _cd;
    int _currentAmmo;

    public bool OnStation;
    bool _hasPita = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetAmmoToMax();
        ammoChanged();
        _loop += Shoot;
        _loop += Aim;
        _loop += AmmoSwitching;
        _loop += DrawProjection;
        offset.m_Offset.Set(0, 0, NotAimingFOV);
    }

    // Update is called once per frame
    void Update()
    {
        _loop?.Invoke();
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, Mathf.Infinity))
        {
            barrel.LookAt(hit.point);
            if (hit.distance < 6)
            {
                Interactable interact = hit.transform.GetComponent<Interactable>();
                if (interact != null)
                {
                    if (!OnStation)
                    {
                        //Info.text = interact.Info;
                        infoUpdate?.Invoke(interact.Info);
                        OnUse = interact.Use;
                    }
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
            infoUpdate?.Invoke(string.Empty);
            barrel.LookAt(cam.position + cam.forward * 200);
        }

        if (Input.GetMouseButton(0) && _cd <= 0 && !OnStation)
        {
            if (_hasPita && isAiming)
            {
                PitaShoot();
            }
            else
            {
                 if (CurrentAmmoType.CurrentAmmo > 0)
                 {
                        GameObject bullet = ObjectPooler.Instance.SpawnFromPool(CurrentAmmoType.AmmoTag, barrel.position, barrel.rotation);
                        _cd = CoolDown;         
                        CurrentAmmoType.CurrentAmmo--;
                        ammoChanged();
                }
                 else
                 {
                     //play the empty gun sound, if the sound is not playing already.
                 }
            }
        }

        if (_cd > 0)
        {
            _cd -= Time.deltaTime;

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnUse?.Invoke(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && OnStation)
        {
            OnUse?.Invoke(gameObject);
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

        CurrentFOV = offset.m_Offset.z;
        if (isAiming)
        {
            if (CurrentFOV < AimingFOV)
            {
                offset.m_Offset.Set(0, 0, CurrentFOV += FovChangingSpeed * Time.deltaTime);
            }
            else { offset.m_Offset.Set(0, 0, AimingFOV); }

        }
        else
        {
            if (CurrentFOV > NotAimingFOV)
            {
                offset.m_Offset.Set(0, 0, CurrentFOV -= FovChangingSpeed * Time.deltaTime);
            }
            else { offset.m_Offset.Set(0, 0, NotAimingFOV); ammoChanged(); }
        }
    }

    void AmmoSwitching()
    {
        if (AmmoTypes.Count > 1)
        {
            if (_hasPita && isAiming)
            {
                lineRenderer.material = PitaTrajectoryMaterial;
                OnPitaAim?.Invoke(_currentPita);
            }
            else
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    _currentAmmo++;
                    if (_currentAmmo > AmmoTypes.Count - 1)
                    {
                        _currentAmmo = 0;
                    }
                    ammoChanged();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    _currentAmmo--;
                    if (_currentAmmo < 0)
                    {
                        _currentAmmo = AmmoTypes.Count - 1;
                    }
                    ammoChanged();
                }
            }
        }
    }

    void ammoChanged()
    {
        CurrentAmmoType = AmmoTypes[_currentAmmo];
        lineRenderer.material = CurrentAmmoType.TrajectoryMaterial;
        OnSwitchWeapon?.Invoke(CurrentAmmoType);
    }
    void DrawProjection()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = Mathf.CeilToInt(linepoints / timeBetweenPoints) + 1;
        Vector3 StartPosition = barrel.position;
        Vector3 StartVelocity = 100 * barrel.forward / 1;
        int i = 0;
        lineRenderer.SetPosition(i, StartPosition);
        for (float time = 0; time < linepoints; time += timeBetweenPoints)
        {
            i++;
            Vector3 point = StartPosition + time * StartVelocity;
            point.y = StartPosition.y + StartVelocity.y * time + (Physics.gravity.y / 2f * time * time);
            lineRenderer.SetPosition(i, point);
            Vector3 lastPostion = lineRenderer.GetPosition(i - 1);
            if (Physics.Raycast(lastPostion, (point - lastPostion).normalized,
                out RaycastHit hit, (point - lastPostion).magnitude, layer))
            {
                lineRenderer.SetPosition(i, hit.point);
                lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }
    void StoppedHoveringStation()
    {
        infoUpdate?.Invoke(string.Empty);
        if (OnStation) { OnUse?.Invoke(gameObject); }
        else OnUse = null;
    }

    /*
    public void ToggleUsingStation()
    {
        OnStation = !OnStation;
        cinemachine.enabled = !cinemachine.enabled;
        tpm.enabled = !tpm.enabled;
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = !Cursor.visible;
        infoUpdate?.Invoke(string.Empty);
    }
    */ // replaced due to causing too many bugs.

    public void StartUsingStation()
    {
        OnStation = true;
        cinemachine.enabled = false;
        tpm.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        infoUpdate?.Invoke(string.Empty);
    }

    public void StopUsingStation()
    {
        OnStation = false;
        cinemachine.enabled = true;
        tpm.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ResetAmmoToMax()
    {
        for (int i = 0; i < 3; i++)
        {
            AmmoTypes[i].CurrentAmmo = AmmoTypes[i].MaxAmmo;
        }
    }
    void PitaShoot()
    {
        GameObject pita = ObjectPooler.Instance.SpawnFromPool("Pita", barrel.position, barrel.rotation);
        _cd = CoolDown;
       Pita a = pita.GetComponent<Pita>();
        List<BuildOrder.Fillers> temp = new List<BuildOrder.Fillers>(_currentPita);
        a.Ingridients = temp;
        _currentPita.Clear();
        _hasPita = false;
        ammoChanged();
    }

    public void SetPita(List<BuildOrder.Fillers> f)
    {
        _currentPita = f;
        _hasPita = true;
        StoppedHoveringStation();
    }
}
