using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    private Gun _gun => player.GetComponent<Gun>();
    private BuildOrder _bo => player.GetComponent<BuildOrder>();

    GameManager _gm;
    LevelTimer _lt;

    [SerializeField] List<Image> Fillers;

    [SerializeField] private TMP_Text         Info;
    [SerializeField] private TMP_Text         Ammo;
    [SerializeField] private TMP_Text         MoneyText;
    [SerializeField] private TMP_Text         Timer;
    [SerializeField] private Image            VictoryScreen;
    [SerializeField] private Image            LoseScreenUi;
    [SerializeField] private Image            TazdokHp;
    [SerializeField] private List<GameObject> _endLevelReset;

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
        _lt.OnUpdateTimer    += UpdateTimer;
        _gm.OnVictoryScreen  += () => VictoryScreen.gameObject.SetActive(true);
        _gm.OnLoseScreen     += LoseScreen;
        _gm.OnEndLevel       += EndLevel;

        UpdateMoney();
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

    void UpdateTimer(int seconds, int minutes)
    {
        string S, M;

        if (seconds < 10) S = $"0{seconds}";
        else S = $"{seconds}";

        if (minutes < 10) M = $"0{minutes}";
        else M = $"{minutes}";

        Timer.text = $"Time Left: {M}:{S}";
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
            case 3:
                TazdokHp.fillAmount = 1;
                break;
            case 2:
                TazdokHp.fillAmount = 0.745f;
                break;
            case 1:
                TazdokHp.fillAmount = 0.58f;
                break;
            case 0:
                TazdokHp.fillAmount = 0.415f;
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
