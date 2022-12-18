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
              [SerializeField] float AimingFOV        = 3;
              [Tooltip("Field of view")]
              [SerializeField] float NotAimingFOV     = 0;
              [Tooltip("The seed of the transition from normal to aiming field of view")]
              [SerializeField] float FovChangingSpeed = 12;
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

    [SerializeField] TMP_Text Info;
    //Event
    Action _loop;
    public Action<Gun> OnUse;

    [SerializeField] Material[] _projectionColors = new Material[3];

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
            Info.text = string.Empty;
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
        if (Input.GetKeyDown(KeyCode.Escape) && OnStation)
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
            else { offset.m_Offset.Set(0, 0, NotAimingFOV); }
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
            switch (CurrentAmmo)
            {
                case "Falafel": lineRenderer.material = _projectionColors[0];  break;
                case "Fries": lineRenderer.material = _projectionColors[1]; break;
                case "Eggplant": lineRenderer.material = _projectionColors[2]; break;

                default: break;
            }
        }
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
        Info.text = string.Empty;
        if (OnStation) { OnUse?.Invoke(this); }
        else OnUse = null;
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
