using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator crossFade;
    // public Animator musicCrossFade;

    public event Action<Scene> LoadingScene;

#if UNITY_EDITOR
    [NaughtyAttributes.Button("Next Scene Test")]
    public void NextSceneTestButton()
    {
        if (!Application.isPlaying) return;
        NextScene();
    }
#endif

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("SceneLoader").Count() > 1)
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void NextScene()
    {
        var scene = SceneManager.GetActiveScene();
        int nextLevelBuildIndex = scene.buildIndex + 1 % SceneManager.sceneCount;

        StartCoroutine(LoadScene(nextLevelBuildIndex));
    }

    public void Goto(int index)
    {
        StartCoroutine(LoadScene(index));
    }

    IEnumerator LoadScene(int i)
    {
        crossFade.Play("CrossFade");

        yield return new WaitForSeconds(.3f);

        SceneManager.LoadSceneAsync(i).completed += _ =>
        {
            LoadingScene?.Invoke(SceneManager.GetSceneAt(i));
        };
    }
}
