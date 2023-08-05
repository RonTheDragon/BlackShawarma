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
    [SerializeField] float AimingFOV = 20;
    [Tooltip("Field of view")]
    [SerializeField] float NotAimingFOV = 40;
    [Tooltip("The speed of transition from normal to aiming field of view")]
    [SerializeField] float FovChangingSpeed = 12;
    [Tooltip("The current field of view")]
    [ReadOnly][SerializeField] float CurrentFOV;
    [Tooltip("Is the character aiming or not")]
    [ReadOnly][SerializeField] bool isAiming;
    [Tooltip("Player Coffee buff")]
    private bool _coffeeBuff = false;
    [Tooltip("Too Close To Aim At")]
    [SerializeField] private float _tooClose = 3;
    [Tooltip("Too Close To Aim At")]
    [SerializeField] private float _aimAtSpeed = 3;
    [Tooltip("Sensitivity When Aiming Multiply")]
    [SerializeField] private float _aimSensitivityMult = 0.5f;
    public bool CantUseStations = false;

    [Header("Ammo Switching")]
    [Tooltip("The current amount of ammo")]
    [ReadOnly] public SOAmmoType CurrentAmmoType;
    [Tooltip("The types of ammo")]
    public List<SOAmmoType> AmmoTypes;

    [Header("References")]
    [Tooltip("Reference to the point where projectiles spawn")]
    [SerializeField] Transform barrel;
    [Tooltip("Camera reference")]
    [SerializeField] Transform cam;
    [SerializeField] private List<Transform> _ropePositions;

    [SerializeField] private Transform _aimAt;
    [SerializeField] private ParticleSystem _chiliParticle;
    //[SerializeField] private float _aimAtSpeed;

    [Tooltip("Reference to the cinemachine")]
    public CinemachineVirtualCamera cinemachine;

    public Transform CursorHand;

    private ThirdPersonMovement tpm => GetComponent<ThirdPersonMovement>();
    private GameManager _gm => GameManager.Instance;
    private AudioManager _am => AudioManager.instance;

    private CinemachineImpulseSource _cis => GetComponent<CinemachineImpulseSource>();

    private BuildOrder _buildOrder => GetComponent<BuildOrder>();
   // private LevelTimer _levelTimer => _gm.GetComponent<LevelTimer>();

    private LevelManager _levelManager => _gm.GetComponent<LevelManager>();

    [Header("Projection")]
    [SerializeField]
    [Range(10, 100)]
    private int linepoints = 25;

    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float timeBetweenPoints = 0.1f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LineRenderer _rope;
    [SerializeField] LayerMask layer;

    [Header("HoldToUse")]
    [ReadOnly][SerializeField] private float _heldDuration = 0;

    private float _outOfAmmoShowTime;

    //[SerializeField] TMP_Text Info;
    //Event
    Action _loop;

    public Action<string> infoUpdate;
    public Action<string> infoUpdateNA;

    public Action<int> OutOfAmmo;

    public Action<GameObject> OnUse;

    public Action OnExit;

    public Action<int> OnHasPitaChanging;

    public Action<float> OnHold;

    public Action<SOAmmoType> OnSwitchWeapon;

    public Action<List<BuildOrder.Fillers>> OnPitaAim;

    public Action OnLafaAim;

    public Action OnLafaShoot;

    List<BuildOrder.Fillers> _currentPita = new List<BuildOrder.Fillers>();

    [SerializeField] Material PitaTrajectoryMaterial;

    [SerializeField] private float _pitaKnockback;
    [SerializeField] private float _recoil = 10;

    [SerializeField] private Animator _gunAnimator;
    [SerializeField] private GameObject _pitaModel;
    [SerializeField] private GameObject _lafaModel;

    [SerializeField] private ParticleSystem _coffeeParticles;


    //Private 
    float _cd;
    int _currentAmmo;

    public bool UsingUI;
    bool _hasPita = false;
    bool _hasLafa = false;

    void Start()
    {
        
        cinemachine.m_Lens.FieldOfView = NotAimingFOV;
        ResetAmmoToMax();
        StartUsingStation();

        _loop += Shoot;
        _loop += UseStationRaycast;
        _loop += Aim;
        _loop += AmmoSwitching;
        _loop += DrawRope;
        _loop += DrawCursor;
        _loop += OutOfAmmoTimer;
        //_loop += DrawProjection;

        _gm.OnVictoryScreen += StartUsingStation;
        _gm.OnLoseScreen += StartUsingStation;
        _gm.OnStartLevel += ClearOnStart;
        _levelManager.OnSetUpLevel += RefillAll;

        _currentAmmo = 0;
        ammoChanged();
        ChiliParticles(false);
        SetCoffee(false);
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
        if (Physics.Raycast(cam.position + cam.forward * _tooClose, cam.forward, out hit, Mathf.Infinity, _gm.NotPlayerLayer))
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
            if (_hasLafa && isAiming)
            {
                LafaShoot();
            }
            else if (_hasPita && isAiming)
            {
                PitaShoot();
            }
            else
            {
                if (CurrentAmmoType.CurrentAmmo > 0)
                {
                    
                    StartCoroutine("ShootDelay");
                    _am.PlayOneShot(FMODEvents.instance.shooting, transform.position);
                }
                
                else
                {
                    //play the empty gun sound, if the sound is not playing already.
                    
                    OutOfAmmo?.Invoke(_currentAmmo);
                    _outOfAmmoShowTime = 1;
                    _am.PlayOneShot(FMODEvents.instance.OutOfAmmo, transform.position);
                    _cd = _coffeeBuff ? CoffeeCD : CoolDown;
                   
                }
            }

            tpm.StopFreeRoaming();
        }

        if (_cd > 0)
        {
            _cd -= Time.deltaTime;

        }
    }

    private System.Collections.IEnumerator ShootDelay()
    {
        _cd = _coffeeBuff ? CoffeeCD : CoolDown;
        yield return new WaitForSeconds(0.01f);
        yield return null;

        GameObject bullet = ObjectPooler.Instance.SpawnFromPool(CurrentAmmoType.AmmoTag, barrel.position, barrel.rotation);
        CurrentAmmoType.CurrentAmmo--;
        ammoChanged();
        float r = isAiming ? _recoil / 3 : _recoil;
        tpm.AddLookTorque(Vector2.down, r);
        _cis.GenerateImpulse(r);
        ShootImpact();
        _gm.OnAmmoUpdate?.Invoke();
    }

    private void AimAt(Vector3 pos)
    {
        barrel.LookAt(pos);
        _aimAt.position = Vector3.MoveTowards(_aimAt.position, pos, Vector3.Distance(_aimAt.position, pos) * _aimAtSpeed * Time.deltaTime);
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
                    interact.UpdateInfo();

                    if (!UsingUI && !interact.NotActive)
                    {
                        infoUpdate?.Invoke(interact.Info);
                        if (!(interact is HoldInteractable))
                        {
                            OnUse = interact.Use;
                            

                        }
                        else
                        {
                            HoldInteractable hold = interact as HoldInteractable;
                            if (!hold.HoldToUse)
                            {
                                OnUse = interact.Use;
                            }
                            else if (Input.GetKey(KeyCode.E))
                            {
                                _heldDuration += Time.deltaTime;
                                if (hold.UseDuration < _heldDuration)
                                {
                                    hold.Use(gameObject);
                                    StoppedHoveringStation();
                                }
                                OnHold?.Invoke(_heldDuration / hold.UseDuration);
                            }
                            else
                            {
                                StoppedHoveringStation();
                                infoUpdate?.Invoke(interact.Info);
                            }
                        }
                    }
                    if (interact.NotActive) { ShowNotActiveInfo(interact.NotActiveInfo); }

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

        if (Input.GetKeyDown(KeyCode.E) && Time.timeScale > 0 && cam.gameObject.activeSelf)
        {
            OnUse?.Invoke(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > 0 && cam.gameObject.activeSelf)
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
            else { cinemachine.m_Lens.FieldOfView = AimingFOV; }

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
            if (_hasLafa && isAiming)
            {
                OnLafaAim?.Invoke();
            }
            else if (_hasPita && isAiming)
            {
                //lineRenderer.material = PitaTrajectoryMaterial;
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
                    _currentAmmo = 1;
                    ammoChanged();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    _currentAmmo = 2;
                    ammoChanged();
                }
            }
        }
    }

    public void RefillAll()
    {
        foreach (SOAmmoType sat in AmmoTypes)
        {
            sat.CurrentAmmo = sat.MaxAmmo;
        }
        _buildOrder.FillAll();
        _buildOrder.Trash();
        _gm.OnAmmoUpdate?.Invoke();
    }

    void ammoChanged()
    {
        CurrentAmmoType = AmmoTypes[_currentAmmo];
        //lineRenderer.material = CurrentAmmoType.TrajectoryMaterial;
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
        for (int i = 0; i < _ropePositions.Count; i++)
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
        _heldDuration = 0;
        OnHold?.Invoke(0);
        infoUpdate?.Invoke(string.Empty);
        if (UsingUI) { OnUse?.Invoke(gameObject); }
        else OnUse = null;
    }

    private void ShowNotActiveInfo(string info)
    {
        _heldDuration = 0;
        OnHold?.Invoke(0);
        infoUpdateNA?.Invoke(info);
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
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.Counter, transform.position);

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
        _gm.OnAmmoUpdate?.Invoke();
    }
    private void PitaShoot()
    {
        _hasPita = false;
        OnHasPitaChanging?.Invoke(0);
        _cd = CoolDown;
        _gunAnimator.SetTrigger("Hamsa");
        StartCoroutine("PitaShootDelay");
        _am.PlayOneShot(FMODEvents.instance.shootingPita, transform.position);
    }

    private void LafaShoot()
    {
        _hasLafa = false;
        OnLafaShoot?.Invoke();
        OnHasPitaChanging?.Invoke(_hasPita? 1:0);
        _cd = CoolDown;
        _gunAnimator.SetTrigger("Hamsa");
        StartCoroutine("LafaShootDelay");
    }

    private void ClearOnStart()
    {
        _hasPita = false;
        _hasLafa = false;
        _pitaModel.SetActive(false);
        _lafaModel.SetActive(false);
        OnLafaShoot?.Invoke();
        OnHasPitaChanging?.Invoke(0);
    }

    private System.Collections.IEnumerator PitaShootDelay()
    {
        yield return new WaitForSeconds(0.17f);
        _pitaModel.SetActive(false);
        GameObject pita = ObjectPooler.Instance.SpawnFromPool("Pita", barrel.position, barrel.rotation);
        tpm.AddForce(-transform.forward, _pitaKnockback * _currentPita.Count);
        tpm.AddLookTorque(Vector2.down, _recoil * _currentPita.Count);
        _cis.GenerateImpulse(_recoil * _currentPita.Count);
        Pita a = pita.GetComponent<Pita>();
        List<BuildOrder.Fillers> temp = new List<BuildOrder.Fillers>(_currentPita);
        a.Ingridients = temp;
        _currentPita.Clear();
        ammoChanged();
    }
    private System.Collections.IEnumerator LafaShootDelay()
    {
        yield return new WaitForSeconds(0.17f);
        _lafaModel.SetActive(false);
        ObjectPooler.Instance.SpawnFromPool("Lafa", barrel.position, barrel.rotation);
        tpm.AddForce(-transform.forward, _pitaKnockback * 4);
        tpm.AddLookTorque(Vector2.down, _recoil * 4);
        _cis.GenerateImpulse(_recoil * 4);
        if (_hasPita)
        {
            _pitaModel.SetActive(true);
        }
    }

    public List<BuildOrder.Fillers> GetPita()
    {
        return _currentPita;
    }

    public void SetPita(List<BuildOrder.Fillers> f)
    {
        _currentPita = f;
        _pitaModel.SetActive(!_hasLafa);
        _hasPita = true;
        OnHasPitaChanging?.Invoke(_hasLafa ? 2 : 1);
        StoppedHoveringStation();
    }
    public void SetLafa(bool lafa)
    {
        _hasLafa = lafa;
        _pitaModel.SetActive(false);
        _lafaModel.SetActive(true);
        OnHasPitaChanging?.Invoke(2);
    }


    public void ChiliParticles(bool on)
    {
        if (on)
        {
            _chiliParticle.Play();
        }
        else
        {
            _chiliParticle.Stop();
        }
    }

    public void SetCoffee(bool coffee)
    {
        _coffeeBuff = coffee;
        if (coffee)
        {
            _coffeeParticles.Play();
        }
        else
        {
            _coffeeParticles.Stop();
        }
    }

    public void SetCoffeeCooldown(float cooldown)
    {
        CoffeeCD = cooldown;
    }

    private void OutOfAmmoTimer()
    {
        if (_outOfAmmoShowTime > 0)
        {
            _outOfAmmoShowTime -= Time.deltaTime;
        }
        else if (_outOfAmmoShowTime <0)
        {
            _outOfAmmoShowTime = 0;
            OutOfAmmo?.Invoke(-1);
        }
    }

}
