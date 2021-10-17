using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager instance;

    public FMODUnity.EventReference Game;
    public FMOD.Studio.EventInstance gameState;

    public FMODUnity.EventReference Title;
    public FMOD.Studio.EventInstance titleState;

    public FMODUnity.EventReference Death;
    public FMOD.Studio.EventInstance deathState;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        if (GameObject.FindGameObjectsWithTag("MusicManager").Count() > 1)
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    void Start() {
        gameState = FMODUnity.RuntimeManager.CreateInstance(Game);
        titleState = FMODUnity.RuntimeManager.CreateInstance(Title);
        deathState = FMODUnity.RuntimeManager.CreateInstance(Death);
    }

    public void StartGameMusic() {
        gameState.start();
        titleState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        deathState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StartTitleMusic() {
        gameState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        titleState.start();
        deathState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StartDeathMusic() {
        gameState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        titleState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        deathState.start();
    }


}
