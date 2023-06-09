using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BuildOrder;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    private Gun _gun => player.GetComponent<Gun>();
    private ThirdPersonMovement _movement => player.GetComponent<ThirdPersonMovement>();
    private BuildOrder _bo => player.GetComponent<BuildOrder>();

    private Shop _shop => GetComponent<Shop>();

    GameManager _gm;
    LevelTimer _lt;
    Tutorial _tutorial;

    [SerializeField] private List<Image> Fillers;
    [SerializeField] private List<GameObject> InsidePitaFiller;

    [SerializeField] private TMP_Text         Info;
    [SerializeField] private TMP_Text         Ammo;
    [SerializeField] private TMP_Text         MoneyText;
    [SerializeField] private TMP_Text         _loseScreenScore;

    [SerializeField] private Image            _holdInducator;

    [Header("Timer")]
    [SerializeField] private Image            _cigar;
    [SerializeField] private RectTransform    _cigarFlame;
                     private float _fullTime;   

    [SerializeField] private Image            VictoryScreen;
    [SerializeField] private Image            LoseScreenUi;
    [SerializeField] private Image            TazdokHp;
    [SerializeField] private List<GameObject> _endLevelReset;
    [SerializeField] private GameObject       _pauseMenu;
    [SerializeField] private Image            _enemyInfoUi;

    [Header("Maximized Order")]
    [SerializeField] private GameObject _baseMaximizedOrder;
    [SerializeField] private Image _orderPanel;
    [SerializeField] private Image _orderProfile;
    [SerializeField] private TMP_Text _orderNumber;
    [SerializeField] private Transform        _maximizedOrder;
    [SerializeField] private Image _fillBar;
    [SerializeField] private Image _foodBG;
    private SideOrderUI _sideOrderMaximized;

    [Header("Stamina")]
    [SerializeField] private GameObject TazdokStamina;
    [SerializeField] private Image TazdokStaminaBar;

    [Header("The Ammo Panel")]
    [SerializeField] private Image _eggplant;
    [SerializeField] private Image _fries;
    [SerializeField] private Image _falafel;
    [SerializeField] private Image _gunWithPita;
    [SerializeField] private List<GameObject> _loadedPitaFillers;
    [SerializeField] private Transform[] _ammoCounters = new Transform[3];

    [SerializeField] private GameObject _shootAmmoPanel;
    [SerializeField] private GameObject _shootPitaPanel;

    [Header("Combo")]
    [SerializeField] private GameObject _comboPanel;
    [SerializeField] private Image _comboTimer;
    [SerializeField] private TMP_Text _currentCombo;

    [Header("Shop")]
    [SerializeField] private GameObject[] _openOnShop;
    [SerializeField] private GameObject[] _closeOnShop;


    private Action _loop;
    //bool isEnemyInfoOpen = false;

    private void Awake()
    {
        OpenShop(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        _gm = GameManager.Instance;
        _lt = _gm.GetComponent<LevelTimer>();
        _tutorial = _gm.GetComponent<Tutorial>();

        _gm.UpdateMoney      += UpdateMoney;
        _gm.UpdateTazdokHp   += UpdateTazdokHPUI;
        _gun.infoUpdate      += UpdateInfo;
        _gun.OnSwitchWeapon  += SwitchAmmoType;
        _gun.OnPitaAim       += SwitchToPita;
        _gun.OnHasPitaChanging += HasPitaChange;
        _gun.OnExit          += OpenPauseMenu;
        _gun.OnHold          += HoldUI;
        _bo.OnUseIngridients += UpdateIngridients;
        _bo.OnPitaUpdate     += PitaUpdate;
        _lt.OnSetTimer       += SetTimer;
        _gm.OnVictoryScreen  += () => VictoryScreen.gameObject.SetActive(true);
        _gm.OnLoseScreen     += LoseScreen;
        _gm.OnEndLevel       += EndLevel;
        _gm.OnOrderMaximize  += SetMaximizedOrder;
        _tutorial.FreezeTimer += FreezeTimer;
        _loop += OpenEnemyInfo;
        _loop += SetMaximizedOrderBar;
        _gm.OnAmmoUpdate += UpdateAmmoCounter;
        _gm.CM.AddEvent += ComboIncrease;
        _gm.CM.ResetEvent += ResetCombo;
        _gm.CM.TimerEvent += ComboTimer;
        _movement.OnStamina += StaminaUI;
        UpdateMoney();
    }

    private void Update()
    {
        _loop?.Invoke();
    }

    void UpdateInfo(string info)
    {
        Info.text = info;
    }

    void SwitchAmmoType(SOAmmoType a)
    {

        Ammo.color = a.AmmoColor;
        Ammo.text  = $"{a.CurrentAmmo}/{a.MaxAmmo}";
        _shootAmmoPanel.SetActive(true);
        _shootPitaPanel.SetActive(false);

        _eggplant.enabled = false;
        _fries.enabled = false;
        _falafel.enabled = false;

        switch (a.FoodType)
        {
            case Edible.Food.Eggplant: 
                _eggplant.enabled= true;
                break;
            case Edible.Food.Fries:
                _fries.enabled = true;
                break;
            case Edible.Food.Falafel:
                _falafel.enabled = true;
                break;
        }
    }

    private void HasPitaChange(bool hasPita)
    {
        _gunWithPita.enabled = hasPita;
    }

    void SwitchToPita(List<BuildOrder.Fillers> pita)
    {
        PitaEdit(ref _loadedPitaFillers, pita);
        _shootAmmoPanel.SetActive(false);
        _shootPitaPanel.SetActive(true);
    }

    void UpdateMoney()
    {
        MoneyText.text =_gm.GetMoney().ToString();
    }

    void UpdateIngridients(List<Ingredient> ingredients)
    {
        for (int i = 0; i < Fillers.Count; i++)
        {
            Fillers[i].fillAmount = (float)ingredients[i].CurrentAmount / (float)ingredients[i].MaxAmount;
        }
    }

    private void PitaUpdate(List<Fillers> pita)
    {
        PitaEdit(ref InsidePitaFiller, pita);
    }

    private void PitaEdit(ref List<GameObject> p, List<Fillers> pita)
    {
        foreach (GameObject g in p)
        {
            g.SetActive(false);
        }

        foreach (Fillers f in pita)
        {
            switch (f)
            {
                case BuildOrder.Fillers.Humus:
                    p[0].SetActive(true);
                    break;
                case BuildOrder.Fillers.Pickles:
                    p[1].SetActive(true);
                    break;
                case BuildOrder.Fillers.Cabbage:
                    p[2].SetActive(true);
                    break;
                case BuildOrder.Fillers.Onions:
                    p[3].SetActive(true);
                    break;
                case BuildOrder.Fillers.Salad:
                    p[4].SetActive(true);
                    break;
                case BuildOrder.Fillers.Spicy:
                    p[5].SetActive(true);
                    break;
                case BuildOrder.Fillers.Amba:
                    p[6].SetActive(true);
                    break;
                case BuildOrder.Fillers.Thina:
                    p[7].SetActive(true);
                    break;
            }
        }
    }

    private void UpdateCigar()
    {
        _lt.TimeLeft -= Time.deltaTime;
        float fa = 1 -(_lt.TimeLeft / _fullTime);
        _cigar.fillAmount = Mathf.Lerp(1, 0.28f, fa);
        _cigarFlame.localPosition = new Vector3(Mathf.Lerp(510.4f, 205.8f, fa), _cigarFlame.localPosition.y, 0);
        if (_lt.TimeLeft < 0) { _lt.OnTimerDone?.Invoke(); _loop -= UpdateCigar; }
    }

    private void SetTimer(float fullTime)
    {
        _loop += UpdateCigar;
        _fullTime = fullTime;
        _cigar.fillAmount = 1;
        _cigarFlame.localPosition = new Vector3(510.4f, _cigarFlame.localPosition.y, 0);
    }

    private void FreezeTimer(bool freeze)
    {
        if (freeze)
        {
            _loop -= UpdateCigar;
        }
        else
        {
            _loop += UpdateCigar;
        }
    }

    private void ComboIncrease(int combo)
    {
        _currentCombo.text = combo.ToString();
        _comboTimer.fillAmount = 1;
        _comboPanel.SetActive(true);
    }

    private void ResetCombo()
    {
        _comboPanel.SetActive(false);
    }

    private void ComboTimer(float f)
    {
        _comboTimer.fillAmount = f;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    
    public void LoseScreen()
    {
        if (_gm.HappyCustomers > 1)
        {
            _loseScreenScore.text = $"Well... at least you managed to satisfy {_gm.HappyCustomers} customers today.";
        }
        else if (_gm.HappyCustomers == 1)
        {
            _loseScreenScore.text = $"Well... at least you managed to satisfy a customer today.";
        }
        else
        {
            _loseScreenScore.text = $"Well... good luck next time";
        }

        LoseScreenUi.gameObject.SetActive(true);
    }
    public void UpdateTazdokHPUI()
    {
        switch (GameManager.Instance.GetTazdokHp())
        {
            case 4:
                TazdokHp.fillAmount = 1;
                break;
            case 3:
                TazdokHp.fillAmount = 0.75f;
                break;
            case 2:
                TazdokHp.fillAmount = 0.50f;
                break;
            case 1:
                TazdokHp.fillAmount = 0.25f;
                break;
            case 0:
                TazdokHp.fillAmount = 0;
                break;
           
            default:
                break;
        }
    }
    private void OpenEnemyInfo()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Time.timeScale != 0)
        { 
          _enemyInfoUi.gameObject.SetActive(!_enemyInfoUi.gameObject.activeSelf);
        }
        
       
    }
    //ui.Fillers, ui.GetPanel(), ui.GetPfp(),ui.GetFoodBG(),ui.OnUpdateBar,ui.GetNumber()
    private void SetMaximizedOrder(SideOrderUI ui)
    {
        for (int i = 0; i < _maximizedOrder.childCount; i++)
        {
            _maximizedOrder.GetChild(i).gameObject.SetActive(false);
        }
        if (ui == null) { _baseMaximizedOrder.SetActive(false); _sideOrderMaximized = null; return; }

        _baseMaximizedOrder.SetActive(true);
        _orderPanel.sprite = ui.GetPanel();
        _orderProfile.sprite = ui.GetPfp();
        _orderNumber.text = ui.GetNumber().ToString();
        _foodBG.sprite = ui.GetFoodBG();
        _sideOrderMaximized = ui;

        for (int i = 0; i < ui.Fillers.Count; i++)
        {
            _maximizedOrder.GetChild(i).gameObject.SetActive(ui.Fillers[i].activeSelf);
        }
    }

    private void UpdateAmmoCounter()
    {
        for (int i = 0; i < _gun.AmmoTypes.Count; i++)
        {
            int ammo = _gun.AmmoTypes[i].CurrentAmmo;
            int maxAmmo = _gun.AmmoTypes[i].MaxAmmo;
            
            for (int j = 0; j < _ammoCounters[i].childCount; j++)
            {
                if (j < maxAmmo)
                {
                    _ammoCounters[i].GetChild(j).gameObject.SetActive(true);
                    _ammoCounters[i].GetChild(j).GetChild(0).gameObject.SetActive(j < ammo);                 
                }
                else { break; }
            }
        }
    }

    private void SetMaximizedOrderBar()
    {
        if (_sideOrderMaximized != null)
        {
            _fillBar.fillAmount = _sideOrderMaximized.GetBar();
        }
    }

    private void StaminaUI(float fill)
    {
        if (fill == 1 || _gun.UsingUI)
        {
            TazdokStamina.SetActive(false);
        }
        else
        {
            float f = 0.15f;
            f +=(fill * 0.7f);
            TazdokStaminaBar.fillAmount = f;
            TazdokStamina.SetActive(true);
        }
    }

    private void HoldUI(float fill)
    {
        _holdInducator.fillAmount = fill;
    }
        
    private void EndLevel()
    {
        _gm.CM.ResetCombo();
        foreach (GameObject item in _endLevelReset)
        {
            item.SetActive(false);
        }
        StartCoroutine("StopTime");
    }

    private IEnumerator StopTime()
    {
        yield return null;
        Time.timeScale = 0;
    }

    private void OpenPauseMenu()
    {
        _pauseMenu.SetActive(true);
        _gun.StartUsingStation();
        Time.timeScale = 0;
    }

    public void ClosePauseMenu()
    {
        Time.timeScale= 1;
        _pauseMenu.SetActive(false);
        _gun.StopUsingStation();
    }

    public void OpenShop(bool Open)
    {
        _shop.HideAllUpgradesUI();
        foreach (GameObject item in _openOnShop)
        {
            item.SetActive(Open);
        }
        foreach (GameObject item in _closeOnShop)
        {
            item.SetActive(!Open);
        }

        Time.timeScale = Open ? 1: 0;
    }

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
             Application.Quit();
#endif
    }
}
