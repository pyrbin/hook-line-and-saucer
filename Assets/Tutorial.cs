using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject MenuObjects;
    public GameObject TutorialObjects;

    public void BackToMenu()
    {
        MenuObjects.SetActive(true);
        TutorialObjects.SetActive(false);
    }

}
