using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> Menus = new List<GameObject>();

    [SerializeField] private Slider _gameSensitivitySlider;

    [SerializeField] private TMP_Text _gameSensitivityPercentDisplay;

    //[SerializeField] private Slider _menuSensitivitySlider;

    //[SerializeField] private TMP_Text _menuSensitivityPercentDisplay;

    //[SerializeField] private Vector2 _menuSensitivity = new Vector2(1, 10);

    private void Start()
    {
        MouseSetUp();
    }

    private void Update()
    {
        EventSystem.current.pixelDragThreshold = 100000000;
    }

    private void MouseSetUp()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!PlayerPrefs.HasKey("Sensitivity")) { PlayerPrefs.SetFloat("Sensitivity", 0.5f); }
        _gameSensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        _gameSensitivityPercentDisplay.text = $"{(int)(_gameSensitivitySlider.value*100)}%";

        //if (!PlayerPrefs.HasKey("Sensitivity2")) { PlayerPrefs.SetFloat("Sensitivity2", 0.5f); }
        //_menuSensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity2");
        //_menuSensitivityPercentDisplay.text = $"{(int)(_menuSensitivitySlider.value * 100)}%";
    }

    public void SceneMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SceneGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetGameSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        _gameSensitivityPercentDisplay.text = $"{(int)(sensitivity * 100)}%";
    }

    //public void SetMenuSensitivity(float sensitivity)
    //{
    //    PlayerPrefs.SetFloat("Sensitivity2", sensitivity);
    //    _menuSensitivityPercentDisplay.text = $"{(int)(sensitivity * 100)}%";

    //    EventSystem.current.pixelDragThreshold = (int)Mathf.Lerp(_menuSensitivity.x, _menuSensitivity.y, PlayerPrefs.GetFloat("Sensitivity2"));
    //}

    public void ShowPanel(int menuNum)
    {
        for (int i = 0; i < Menus.Count; i++)
        {
            Menus[i].gameObject.SetActive(false);
        }
        Menus[menuNum].SetActive(true);
    }
}
