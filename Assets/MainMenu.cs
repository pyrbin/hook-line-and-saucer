using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MenuObjects;
    public GameObject Tutorial;

    void Start() {
        MusicManager.instance.StartTitleMusic();
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
