using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject MainMenu;

    public GameObject HowToPlayMenu;

    public GameObject CreditMenu;


    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Back()
    {
        HowToPlayMenu.SetActive(false);
        CreditMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void ShowHowToMenu()
    {
        MainMenu.SetActive(false);
        HowToPlayMenu.SetActive(true);
    }

    public void ShowCreditMenu()
    {
        MainMenu.SetActive(false);
        CreditMenu.SetActive(true);
    }
}
