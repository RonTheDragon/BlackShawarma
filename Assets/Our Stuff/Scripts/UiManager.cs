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
    private BuildOrder _bo => player.GetComponent<BuildOrder>();

    GameManager _gm;
    LevelTimer _lt;
    Tutorial _tutorial;

    [SerializeField] private List<Image> Fillers;
    [SerializeField] private List<GameObject> InsidePitaFiller;

    [SerializeField] private TMP_Text         Info;
    [SerializeField] private TMP_Text         Ammo;
    [SerializeField] private TMP_Text         MoneyText;

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

    [Header("The Ammo Panel")]
    [SerializeField] private Image _eggplant;
    [SerializeField] private Image _fries;
    [SerializeField] private Image _falafel;
    [SerializeField] private Image _gunWithPita;
    [SerializeField] private List<GameObject> _loadedPitaFillers;

    [SerializeField] private GameObject _shootAmmoPanel;
    [SerializeField] private GameObject _shootPitaPanel;


    private Action _loop;
    //bool isEnemyInfoOpen = false;


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
        _bo.OnUseIngridients += UpdateIngridients;
        _bo.OnPitaUpdate     += PitaUpdate;
        _lt.OnSetTimer       += SetTimer;
        _gm.OnVictoryScreen  += () => VictoryScreen.gameObject.SetActive(true);
        _gm.OnLoseScreen     += LoseScreen;
        _gm.OnEndLevel       += EndLevel;
        _tutorial.FreezeTimer += FreezeTimer;
        _loop += OpenEnemyInfo;
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
        MoneyText.text = "Joobot = " + _gm.GetMoney().ToString() + "�";
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
          _enemyInfoUi.gameObject.SetActive(!_enemyInfoUi.gameObject.activeSelf);
        }
    }

    private void EndLevel()
    {
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
