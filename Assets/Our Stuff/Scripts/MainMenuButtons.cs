using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> Menus = new List<GameObject>();

    [SerializeField] private Slider _sensitivitySlider;

    [SerializeField] private TMP_Text _sensitivityPercentDisplay;

    private void Start()
    {
        MouseSetUp();
    }

    private void MouseSetUp()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!PlayerPrefs.HasKey("Sensitivity")) { PlayerPrefs.SetFloat("Sensitivity", 0.5f); }
        _sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        _sensitivityPercentDisplay.text = $"{(int)(_sensitivitySlider.value*100)}%";
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

    public void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        _sensitivityPercentDisplay.text = $"{(int)(sensitivity * 100)}%";
    }

    public void ShowPanel(int menuNum)
    {
        for (int i = 0; i < Menus.Count; i++)
        {
            Menus[i].gameObject.SetActive(false);
        }
        Menus[menuNum].SetActive(true);
    }
}
