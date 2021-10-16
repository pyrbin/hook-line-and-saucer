using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action StartedFishing;
    public event Action StartedThrowFish;

    public FishCollectArea fishCollectArea;
    public Transform fishProjectilePosition;
    public CameraManager cameraManager;
    public PlayerThrowFish playerThrowFish;
    public DragPower dragPower;
    public HUDManager hudManager;

    public Player player;    

    void Start()
    {
        StartedFishing += () =>
        {
            hudManager.powerBar.gameObject.SetActive(false);
            cameraManager.GoToFishing();
            player.StartFishing();
        };

        StartedThrowFish += () =>
        {     
            hudManager.powerBar.gameObject.SetActive(true);
            dragPower.Activate();
            cameraManager.GoToOverview();
        };

        fishCollectArea.FishCollected += (fish) => OnCollectFish(fish);

        playerThrowFish.FirstThrow += () =>
        {

            cameraManager.GoToTracking(playerThrowFish.Fish.gameObject.transform);
            playerThrowFish.Fish.Parent.Despawned += (stats) =>
            {
                StartFishing();
            };
        };
    }

    void OnCollectFish(Fish fish) {
        StartedThrowFish?.Invoke();
        fish.transform.position = fishProjectilePosition.position;
        player.StartThrowFish(fish);
    }

    void StartFishing() {
        StartedFishing?.Invoke();
    }
}
