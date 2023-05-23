using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //Serializefield 
    [Tooltip("shooting cooldown")]
    [SerializeField] float CoolDown = 0.5f;
    [SerializeField] float CoffeeCD = 0.25f;

    [Header("Aiming")]
    [Tooltip("Field of view while aiming")]
    [SerializeField]           float AimingFOV        = 20;
    [Tooltip("Field of view")]
    [SerializeField]           float NotAimingFOV     = 40;
    [Tooltip("The speed of transition from normal to aiming field of view")]
    [SerializeField]           float FovChangingSpeed = 12;
    [Tooltip("The current field of view")]
    [ReadOnly][SerializeField] float CurrentFOV;
    [Tooltip("Is the character aiming or not")]
    [ReadOnly][SerializeField] bool  isAiming;
    [Tooltip("Player Coffee buff")]
    [SerializeField] public bool _coffeeBuf = false;
    [Tooltip("Too Close To Aim At")]
    [SerializeField] private float _tooClose = 3;
    [Tooltip("Too Close To Aim At")]
    [SerializeField] private float _aimAtSpeed = 3;
    [Tooltip("Sensitivity When Aiming Multiply")]
    [SerializeField] private float _aimSensitivityMult = 0.5f;
    public bool CantUseStations = false;

    [Header("Ammo Switching")]
    [Tooltip("The current amount of ammo")]
    [ReadOnly]public SOAmmoType CurrentAmmoType;
    [Tooltip("The types of ammo")]
    public List<SOAmmoType>     AmmoTypes;

    [Header("Refefrences")]
    [Tooltip("Reference to the point where projectiles spawn")]
    [SerializeField] Transform barrel;
    [Tooltip("Camera reference")]
    [SerializeField] Transform cam;
    [SerializeField] private List<Transform> _ropePositions;

    [SerializeField] private Transform _aimAt;
    //[SerializeField] private float _aimAtSpeed;

    [Tooltip("Reference to the cinemachine")]
    public CinemachineVirtualCamera cinemachine;

    public Transform CursorHand;

    private ThirdPersonMovement     tpm    => GetComponent<ThirdPersonMovement>();
    private GameManager             _gm    => GameManager.Instance;

    private CinemachineImpulseSource _cis => GetComponent<CinemachineImpulseSource>();

    private BuildOrder _buildOrder => GetComponent<BuildOrder>();
    private LevelTimer _levelTimer => _gm.GetComponent<LevelTimer>();

    private LevelManager _levelManager => _gm.GetComponent<LevelManager>();

    [Header("Projection")]
    [SerializeField]
    [Range(10, 100)]
    private int linepoints = 25;

    [SerializeField]
    [Range(0.01f, 0.25f)]
    private          float        timeBetweenPoints = 0.1f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LineRenderer _rope;
    [SerializeField] LayerMask    layer;

    //[SerializeField] TMP_Text Info;
    //Event
    Action _loop;

    public Action<string> infoUpdate;

    public Action<GameObject> OnUse;

    public Action OnExit;

    public Action<bool> OnHasPitaChanging;

    public Action<SOAmmoType> OnSwitchWeapon;

    public Action<List<BuildOrder.Fillers>> OnPitaAim;

    List<BuildOrder.Fillers> _currentPita = new List<BuildOrder.Fillers>();

    [SerializeField] Material PitaTrajectoryMaterial;

    [SerializeField] private float _pitaKnockback;
    [SerializeField] private float _recoil = 10;

    [SerializeField] private Animator _gunAnimator;
    [SerializeField] private GameObject _pitaModel;


    //Private 
    float _cd;
    int   _currentAmmo;

    public bool UsingUI;
    bool _hasPita = false;

    void Start()
    {
        cinemachine.m_Lens.FieldOfView = NotAimingFOV;
        ResetAmmoToMax();
        ammoChanged();
        StartUsingStation();

        _loop += Shoot;
        _loop += UseStationRaycast;
        _loop += Aim;
        _loop += AmmoSwitching;
        _loop += DrawRope;
        _loop += DrawCursor;
        //_loop += DrawProjection;

        _gm.OnVictoryScreen += StartUsingStation;
        _gm.OnLoseScreen+= StartUsingStation;
        _levelManager.OnSetUpLevel += RefillAll;
    }

    void Update()
    {
        _loop?.Invoke();
    }

    private void FixedUpdate()
    {
        DrawRope();
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position+ cam.forward* _tooClose, cam.forward, out hit, Mathf.Infinity,_gm.NotPlayerLayer))
        {
            //barrel.LookAt(hit.point);
            //AimAt(hit.point);
            //_aimAt.position = hit.point;
            AimAt(hit.point);

        }
        else
        {
            //AimAt(cam.position + cam.forward * 200);
            //_aimAt.position = cam.position + cam.forward * 200;
            AimAt(cam.position + cam.forward * 200);
        }

        if (Input.GetMouseButton(0) && _cd <= 0 && !UsingUI) // When shoot
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerFootsteps, Camera.main.transform.position);
            if (_hasPita && isAiming)
            {
                tpm.AddForce(-transform.forward, _pitaKnockback*_currentPita.Count);
                tpm.AddLookTorque(Vector2.down, _recoil * _currentPita.Count);
                _cis.GenerateImpulse(_recoil * _currentPita.Count);
                PitaShoot();
            }
            else
            {
                if (CurrentAmmoType.CurrentAmmo > 0 && !_coffeeBuf)
                {
                    _cd = CoolDown;

                  //  if (!tpm.FreeRoam)
                    {
                        GameObject bullet = ObjectPooler.Instance.SpawnFromPool(CurrentAmmoType.AmmoTag, barrel.position, barrel.rotation);
                        CurrentAmmoType.CurrentAmmo--;
                        ammoChanged();
                        float r = isAiming ? _recoil / 3 : _recoil;
                        tpm.AddLookTorque(Vector2.down, r);
                        _cis.GenerateImpulse(r);
                        ShootImpact();
                    }
                }
                if (CurrentAmmoType.CurrentAmmo > 0 && _coffeeBuf)
                {
                    _cd = CoffeeCD;

                  //  if (!tpm.FreeRoam)
                    {
                        
                        GameObject bullet = ObjectPooler.Instance.SpawnFromPool(CurrentAmmoType.AmmoTag, barrel.position, barrel.rotation);
                        CurrentAmmoType.CurrentAmmo--;
                        ammoChanged();
                        float r = isAiming ? _recoil / 3 : _recoil;
                        tpm.AddLookTorque(Vector2.down, r);
                        _cis.GenerateImpulse(r);
                        ShootImpact();
                    }
                }
                else
                {
                    //play the empty gun sound, if the sound is not playing already.
                }
            }

            tpm.StopFreeRoaming();
        }

        if (_cd > 0)
        {
            _cd -= Time.deltaTime;

        }
    }

    private void AimAt(Vector3 pos)
    {
        barrel.LookAt(pos);
        _aimAt.position = Vector3.MoveTowards(_aimAt.position, pos, Vector3.Distance(_aimAt.position,pos) * _aimAtSpeed * Time.deltaTime);
    }
    private void UseStationRaycast()
    {
        if (CantUseStations) return;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, Mathf.Infinity, _gm.NotPlayerLayer))
        {
            if (hit.distance < 8)
            {
                Interactable interact = hit.transform.GetComponent<Interactable>();
                if (interact != null)
                {
                    if (!UsingUI && !interact.NotActive)
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
        }

        if (Input.GetKeyDown(KeyCode.E) && Time.timeScale>0)
        {
            OnUse?.Invoke(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > 0)
        {
            if (UsingUI)
            {             
              OnUse?.Invoke(gameObject);
            }
            else OnExit?.Invoke();
        }
    }

    private void ShootImpact()
    {
        _gunAnimator.SetTrigger("Play");
    }

    void Aim()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
            tpm.MultiplySensitivity(_aimSensitivityMult);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            tpm.DivideSensitivity(_aimSensitivityMult);
        }

        CurrentFOV = cinemachine.m_Lens.FieldOfView;
        if (isAiming)
        {
            tpm.StopFreeRoaming();
            if (CurrentFOV > AimingFOV)
            {
                cinemachine.m_Lens.FieldOfView = CurrentFOV - FovChangingSpeed * Time.deltaTime;
            }
            else { cinemachine.m_Lens.FieldOfView= AimingFOV; }

        }
        else
        {
            if (CurrentFOV < NotAimingFOV)
            {
                cinemachine.m_Lens.FieldOfView = CurrentFOV + FovChangingSpeed * Time.deltaTime;
            }
            else { cinemachine.m_Lens.FieldOfView = NotAimingFOV; ammoChanged(); }
        }
    }

    void AmmoSwitching()
    {
        if (AmmoTypes.Count > 1 && !UsingUI)
        {
            if (_hasPita && isAiming)
            {
                lineRenderer.material = PitaTrajectoryMaterial;
                OnPitaAim?.Invoke(_currentPita);
            }
            else
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    _currentAmmo++;
                    if (_currentAmmo > AmmoTypes.Count - 1)
                    {
                        _currentAmmo = 0;
                    }
                    ammoChanged();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    _currentAmmo--;
                    if (_currentAmmo < 0)
                    {
                        _currentAmmo = AmmoTypes.Count - 1;
                    }
                    ammoChanged();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    _currentAmmo = 0;
                    ammoChanged();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    _currentAmmo = 2;
                    ammoChanged();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    _currentAmmo = 1;
                    ammoChanged();
                }
            }
        }
    }

    public void RefillAll()
    {
        foreach(SOAmmoType sat in AmmoTypes)
        {
            sat.CurrentAmmo = sat.MaxAmmo;
        }
        _buildOrder.FillAll();
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

    private void DrawRope()
    {
        _rope.enabled = true;
        _rope.positionCount = _ropePositions.Count;
        for (int i =0; i<_ropePositions.Count; i++)
        {
            _rope.SetPosition(i, _ropePositions[i].position);
        }
    }

    private void DrawCursor()
    {
        CursorHand.position = Input.mousePosition;
    }

    void StoppedHoveringStation()
    {
        infoUpdate?.Invoke(string.Empty);
        if (UsingUI) { OnUse?.Invoke(gameObject); }
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
        UsingUI = true;
        cinemachine.enabled = false;
        tpm.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        CursorHand.gameObject.SetActive(true);
        infoUpdate?.Invoke(string.Empty);
    }

    public void StopUsingStation()
    {
        UsingUI = false;
        cinemachine.enabled = true;
        tpm.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CursorHand.gameObject.SetActive(false);
    }

    private void ResetAmmoToMax()
    {
        for (int i = 0; i < 3; i++)
        {
            AmmoTypes[i].CurrentAmmo = AmmoTypes[i].MaxAmmo;
        }
    }
    private void PitaShoot()
    {
        _hasPita = false;
        OnHasPitaChanging?.Invoke(_hasPita);
        _cd = CoolDown;
        _gunAnimator.SetTrigger("Hamsa");
        StartCoroutine("PitaShootDelay");
    }

    private System.Collections.IEnumerator PitaShootDelay()
    {
        yield return new WaitForSeconds(0.17f);
        _pitaModel.SetActive(false );
        GameObject pita = ObjectPooler.Instance.SpawnFromPool("Pita", barrel.position, barrel.rotation);
        Pita a = pita.GetComponent<Pita>();
        List<BuildOrder.Fillers> temp = new List<BuildOrder.Fillers>(_currentPita);
        a.Ingridients = temp;
        _currentPita.Clear();
        ammoChanged();
    }

    public void SetPita(List<BuildOrder.Fillers> f)
    {
        _currentPita = f;
        _pitaModel.SetActive(true);
        _hasPita     = true;
        OnHasPitaChanging?.Invoke(_hasPita);
        StoppedHoveringStation();
    }

    public List<BuildOrder.Fillers> GetPita()
    {
        return _currentPita;
    }
}
