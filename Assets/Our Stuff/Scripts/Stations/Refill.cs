using System;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
public class Refill : MonoBehaviour, Interactable
{
    [SerializeField] SOAmmoType _type;
    [SerializeField] private string _info;

    //[SerializeField] private bool UseOption1;

    //[Header("Option 1")]
    //[SerializeField] private float _turnOffDistance = 3;
    //[SerializeField] private int _perHarvest = 5;
    //[SerializeField] private float _harvestCooldown = 1;
    //private float _currentHarvestCooldown;

    //[Header("Option 2")]
    //[SerializeField] private float _cookingDuration = 3;
    //private float _currentCooking;
    //private bool _cooking;
    //private bool _filled;

    [SerializeField] private float _cookingDuration = 3;
    private float _currentCooking;
    private bool _cooking;
    private StudioEventEmitter emitter;
 // [SerializeField] private int _maxContained;
   //[SerializeField] private int _currentContained;
   //[SerializeField] private float _transferSpeed;
   // private float _currentTransferTime;
   // [SerializeField] private float _transferDistance;

    public string Info { get => _info; set => _info = value; }

    private bool _notActive;

    public bool NotActive { get => _notActive; set => _notActive = value; }
    private Camera PlayerCamera => Camera.main;

    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    [Header("refrences")]
    [SerializeField] private GameObject _ui;
    private Animator _uiAnim => _ui.transform.GetChild(0).GetComponent<Animator>();
    private Image _fillable => _ui.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
    [SerializeField] private Animator _anim;
    [SerializeField] private ParticleSystem _particle;

    public string NotActiveInfo { get => _notActiveInfo; set => _notActiveInfo = value; }
    private string _notActiveInfo;
    [SerializeField] private string _cantUseBecauseFull;
    [SerializeField] private string _cantUseBecauseTimer;
    private float _infoUpdateCooldown;

    //private Transform _playerLocation;
    //private Gun _gun;
    private GameManager _gm;
    private AudioManager _am;


    // private SOAmmoType _selectedFood;

    private void Start()
    {
        _gm = GameManager.Instance;
        _gm.OnPickUpSack += () => _notActive = true;
        _gm.OnPlaceDownSack += () => _notActive = false;
        _gm.OnStartLevel += ResetStation;
        _am = AudioManager.instance;
        emitter = _am.InitializeEventEmitter(FMODEvents.instance.Chipser,this.gameObject);
        emitter.Play();
       // _playerLocation = _gm.Player.transform;
        //_gun = _gm.Player.GetComponent<Gun>();
        //foreach (SOAmmoType i in _gun.AmmoTypes)
        //{
        //    if (i.FoodType == _type.FoodType)
        //    {
        //        _selectedFood = i; break;
        //    }
        //}
    }

    private void Update()
    {
        //FillPlayer();
        Cooking();
        stationUI();
        if (_infoUpdateCooldown > 0)
        {
            _infoUpdateCooldown -= Time.deltaTime;
        }
        //if (UseOption1)
        //{
        //    Working();
        //}
        //else
        //{
        //    Working2();
        //}
    }

    public void Use(GameObject player)
    {
        _used?.Invoke();

        Cook();

        //if (UseOption1)
        //{
        //    Option1(player);
        //}
        //else
        //{
        //    Option2(player);
        //}
    }

    private void stationUI()
    {
        if (PlayerCamera == null) return;
            _ui.transform.LookAt(PlayerCamera.transform.position);
        if (_cooking)
        {
            _fillable.fillAmount = 1 -(_currentCooking / _cookingDuration);
            _uiAnim.SetBool("Empty", false);
            NotActive = true;
        }
        else
        {
            //_fillable.fillAmount = (float)_currentContained / (float)_maxContained;
            _fillable.fillAmount =1;
            _uiAnim.SetBool("Empty", _type.CurrentAmmo == 0);
            NotActive = _type.CurrentAmmo == _type.MaxAmmo;
        }
    }

    private void Cook()
    {
        if (!_cooking && _type.CurrentAmmo < _type.MaxAmmo)
        {
            _currentCooking = _cookingDuration;
            _cooking = true;
            _anim.SetBool("Active", true);
            _uiAnim.SetBool("Empty", false);
            _type.CurrentAmmo = _type.MaxAmmo;
            _gm.OnAmmoUpdate?.Invoke();
            _particle.Play();
            emitter.Stop();
           _am.PlayOneShot(FMODEvents.instance.ChipserUse, gameObject.transform.position);
        }
    }

    private void Cooking()
    {
        if (_cooking)
        {
            
            if (_currentCooking > 0)
            {
                _currentCooking -= Time.deltaTime;
                
            }
            else
            {
                _cooking = false;
                _anim.SetBool("Active", false);
            }
        }
    }


    //private void FillPlayer()
    //{
    //    if (_currentContained == 0 || _currentCooking > 0 || _selectedFood.CurrentAmmo == _selectedFood.MaxAmmo||
    //        Vector3.Distance(transform.position, _playerLocation.position) > _transferDistance)
    //    {
    //        _uiAnim.SetBool("Collecting", false);
    //        return;
    //    }
    //    _uiAnim.SetBool("Collecting", true);

    //    if (_currentTransferTime <= 0)
    //    {
    //        _currentContained--;
    //        _selectedFood.CurrentAmmo++;
    //        _gm.OnAmmoUpdate?.Invoke();
    //        _currentTransferTime = _transferSpeed;
    //    }
    //    else
    //    {
    //        _currentTransferTime -= Time.deltaTime;
    //    }
    //}

    private void ResetStation()
    {
        _cooking = false;
        _currentCooking = 0;
        _anim.SetBool("Active", false);
        _uiAnim.SetBool("Collecting", false);
        _uiAnim.SetBool("Empty", false);
    }

    //private void NeedsRefill2()
    //{
    //    if (_type.CurrentAmmo == 0 || _filled)
    //    {
    //        if (_ui.activeSelf == false) _ui.SetActive(true);
    //        _ui.transform.LookAt(PlayerCamera.transform.position);
    //    }
    //    else if (_ui.activeSelf == true)
    //    {
    //        _ui.SetActive(false);
    //    }
    //}

    //private void Option1(GameObject player)
    //{
    //    if (!_refillerWorking)
    //    {
    //        _playerLocation = player.transform;
    //        _gun = player.GetComponent<Gun>();
    //        _currentHarvestCooldown = _harvestCooldown;
    //        _refillerWorking = true;
    //        _anim.SetBool("Active", true);
    //        foreach (SOAmmoType i in _gun.AmmoTypes)
    //        {
    //            if (i.FoodType == _type.FoodType)
    //            {
    //                _selectedFood = i; break;
    //            }
    //        }
    //    }
    //}

    //private void Option2(GameObject player)
    //{
    //    _gun = player.GetComponent<Gun>();
    //    foreach (SOAmmoType i in _gun.AmmoTypes)
    //    {
    //        if (i.FoodType == _type.FoodType)
    //        {
    //            _selectedFood = i; break;
    //        }
    //    }

    //    if (_filled) 
    //    {
    //        _filled = false;
    //        _selectedFood.CurrentAmmo = _selectedFood.MaxAmmo;
    //        return;
    //    }
    //    else 
    //    {
    //        if (!_cooking)
    //        {
    //            _anim.SetBool("Active", true); _cooking = true; _currentCooking = _cookingDuration;
    //        }
    //    }
    //}

    //private void Working()
    //{
    //    NeedsRefill();

    //    if (!_refillerWorking) return;

    //    if (Vector3.Distance(transform.position, _playerLocation.position)> _turnOffDistance) { _refillerWorking = false; _anim.SetBool("Active", false); return; }

    //    _currentHarvestCooldown -= Time.deltaTime;

    //    if (_selectedFood.CurrentAmmo == _selectedFood.MaxAmmo) {  _refillerWorking = false; _anim.SetBool("Active", false); return; }

    //    if (_currentHarvestCooldown < 0) 
    //    { 
    //        _currentHarvestCooldown = _harvestCooldown;
    //        _selectedFood.CurrentAmmo += _perHarvest;
    //        if (_selectedFood.CurrentAmmo> _selectedFood.MaxAmmo) { _selectedFood.CurrentAmmo = _selectedFood.MaxAmmo; }
    //    }
    //}

    //private void Working2()
    //{
    //    NeedsRefill2();
    //    if (_cooking)
    //    {
    //        _currentCooking -= Time.deltaTime;
    //        if ( _currentCooking < 0)
    //        {
    //            _filled = true; _cooking = false; _anim.SetBool("Active", false);
    //        }
    //    }
    //}

    public void UpdateInfo()
    {
        if (_infoUpdateCooldown <= 0)
        {
            _infoUpdateCooldown = 1;
            if (_type.CurrentAmmo == _type.MaxAmmo)
            {
                _notActiveInfo = _cantUseBecauseFull;
            }
            else if (_currentCooking > 0)
            {
                _notActiveInfo = $"{(int)_currentCooking} {_cantUseBecauseTimer}";
            }
        }
    }
}
