using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject homePage;
    public GameObject levelSelect;

    public void HomePage()
    {
        levelSelect.SetActive(false);
        homePage.SetActive(true);
    }

    public void LevelSelect()
    {
        levelSelect.SetActive(true);
        homePage.SetActive(false);
    }

    public void Level1()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void Level2()
    {
        SceneManager.LoadScene("Level 2");
    }

    public void Level3()
    {
        SceneManager.LoadScene("Level 3");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
