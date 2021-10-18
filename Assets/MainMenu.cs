using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MenuObjects;
    public GameObject Tutorial;

    void Start() {
        if (FMODUnity.RuntimeManager.HasBankLoaded("Master"))
        {
            Debug.Log("Master Bank Loaded");
            MusicManager.instance.StartTitleMusic();
        } else {
            Debug.Log("Bank is not loaded");
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowTutorial()
    {
        MenuObjects.SetActive(false);
        Tutorial.SetActive(true);
    }

}
