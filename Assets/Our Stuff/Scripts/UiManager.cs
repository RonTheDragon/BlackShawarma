using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
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
    [SerializeField] private TMP_Text         Timer;
    [SerializeField] private Image            _cigar;
    [SerializeField] private RectTransform    _cigarFlame;
                     private float _cigarSize;
                     private float _flamePos;
                     private float _fullTime;   

    [SerializeField] private Image            VictoryScreen;
    [SerializeField] private Image            LoseScreenUi;
    [SerializeField] private Image            TazdokHp;
    [SerializeField] private List<GameObject> _endLevelReset;

    private Action _loop;


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
        _bo.OnUseIngridients += UpdateIngridients;
        _bo.OnPitaUpdate     += PitaUpdate;
        _lt.OnUpdateTimer    += UpdateTimer;
        _lt.OnUpdateCigar    += UpdateCigar;
        _lt.OnSetTimer       += SetTimer;
        _loop                += UpdateCigarWithLerp;
        _gm.OnVictoryScreen  += () => VictoryScreen.gameObject.SetActive(true);
        _gm.OnLoseScreen     += LoseScreen;
        _gm.OnEndLevel       += EndLevel;

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

    private void UpdateTimer(int seconds, int minutes)
    {
        string S, M;

        if (seconds < 10) S = $"0{seconds}";
        else S = $"{seconds}";

        if (minutes < 10) M = $"0{minutes}";
        else M = $"{minutes}";

        Timer.text = $"{M}:{S}";
    }

    private void UpdateCigar(float timeLeft)
    {
        float fa = 1 -(timeLeft / _fullTime);
        _cigarSize = Mathf.Lerp(1, 0.28f, fa);
        _flamePos = Mathf.Lerp(528,119,fa);
    }

    private void SetTimer(float fullTime)
    {
        _fullTime = fullTime;
        _cigarSize = 1;
        _flamePos = 528;
        _cigar.fillAmount = 1;
        _cigarFlame.position = new Vector3(528, _cigarFlame.position.y, 0);
    }

    private void UpdateCigarWithLerp()
    {
        float LerpSpeed = 1 / (_fullTime*10);
        _cigar.fillAmount = Mathf.Lerp(_cigar.fillAmount, _cigarSize, LerpSpeed);
        _cigarFlame.position = new Vector3(Mathf.Lerp(_cigarFlame.position.x, _flamePos, LerpSpeed), _cigarFlame.position.y, 0);
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

    private void EndLevel()
    {
        foreach (GameObject item in _endLevelReset)
        {
            item.SetActive(false);
        }
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
