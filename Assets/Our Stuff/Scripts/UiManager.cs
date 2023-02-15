using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject       _exitMenu;
    [SerializeField] private Image            _enemyInfoUi;

    private Action _loop;
    bool isEnemyInfoOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        _gm = GameManager.Instance;
        _lt = _gm.GetComponent<LevelTimer>();

        _gm.UpdateMoney      += UpdateMoney;
        _gm.UpdateTazdokHp   += UpdateTazdokHPUI;
        _gun.infoUpdate      += UpdateInfo;
        _gun.OnSwitchWeapon  += SwitchAmmoType;
        _gun.OnPitaAim       += SwitchToPita;
        _gun.OnExit          += OpenExitMenu;
        _bo.OnUseIngridients += UpdateIngridients;
        _bo.OnPitaUpdate     += PitaUpdate;
        _lt.OnSetTimer       += SetTimer;
        _gm.OnVictoryScreen  += () => VictoryScreen.gameObject.SetActive(true);
        _gm.OnLoseScreen     += LoseScreen;
        _gm.OnEndLevel       += EndLevel;
        UpdateMoney();
    }

    private void Update()
    {
        _loop?.Invoke();
        OpenEnemyInfo();


    }

    void UpdateInfo(string info)
    {
        Info.text = info;
    }

    void SwitchAmmoType(SOAmmoType a)
    {
        Ammo.color = a.AmmoColor;
        Ammo.text  = $"{a.AmmoTag}\nAmmo:\n{a.CurrentAmmo}/{a.MaxAmmo}";
    }

    void SwitchToPita(List<BuildOrder.Fillers> pita)
    {
        Ammo.color = Color.white;
        string txt = "Pita with:";
        foreach (BuildOrder.Fillers f in pita)
        {
            txt += $"\n{f}";
        }
        Ammo.text = txt;
    }
    void UpdateMoney()
    {
        MoneyText.text = "Joobot = " + _gm.GetMoney().ToString() + "¤";
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
        foreach(GameObject g in InsidePitaFiller)
        {
            g.SetActive(false);
        }

        foreach(Fillers f in pita)
        {
            switch (f)
            {
                case BuildOrder.Fillers.Humus:
                    InsidePitaFiller[0].SetActive(true);
                    break;
                case BuildOrder.Fillers.Pickles:
                    InsidePitaFiller[1].SetActive(true);
                    break;
                case BuildOrder.Fillers.Cabbage:
                    InsidePitaFiller[2].SetActive(true);
                    break;
                case BuildOrder.Fillers.Onions:
                    InsidePitaFiller[3].SetActive(true);
                    break;
                case BuildOrder.Fillers.Salad:
                    InsidePitaFiller[4].SetActive(true);
                    break;
                case BuildOrder.Fillers.Spicy:
                    InsidePitaFiller[5].SetActive(true);
                    break;
                case BuildOrder.Fillers.Amba:
                    InsidePitaFiller[6].SetActive(true);
                    break;
                case BuildOrder.Fillers.Thina:
                    InsidePitaFiller[7].SetActive(true);
                    break;
            }
        }
    }

    private void UpdateCigar()
    {
        _lt.TimeLeft -= Time.deltaTime;
        float fa = 1 -(_lt.TimeLeft / _fullTime);
        _cigar.fillAmount = Mathf.Lerp(1, 0.28f, fa);
        _cigarFlame.localPosition = new Vector3(Mathf.Lerp(510.4f, 210.8f, fa), _cigarFlame.localPosition.y, 0);
        if (_lt.TimeLeft < 0) { _lt.OnTimerDone?.Invoke(); _loop -= UpdateCigar; }
    }

    private void SetTimer(float fullTime)
    {
        _loop += UpdateCigar;
        _fullTime = fullTime;
        _cigar.fillAmount = 1;
        _cigarFlame.localPosition = new Vector3(510.4f, _cigarFlame.localPosition.y, 0);
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
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

          _enemyInfoUi.gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            _enemyInfoUi.gameObject.SetActive(false);
        }
    }

    private void EndLevel()
    {
        foreach (GameObject item in _endLevelReset)
        {
            item.SetActive(false);
        }
    }

    private void OpenExitMenu()
    {
        _exitMenu.SetActive(true);
        _gun.StartUsingStation();
        Time.timeScale = 0;
    }

    public void CloseExitMenu()
    {
        Time.timeScale= 1;
        _exitMenu.SetActive(false);
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
