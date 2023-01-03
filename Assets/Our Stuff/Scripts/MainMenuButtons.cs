using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> Menus = new List<GameObject>();

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

    public void ShowMainMenu()
    {
        Menus[0].gameObject.SetActive(true);
        for (int i = 1; i < Menus.Count; i++)
        {
            Menus[i].gameObject.SetActive(false);
        }
        Menus[0].SetActive(true);
    }
}
