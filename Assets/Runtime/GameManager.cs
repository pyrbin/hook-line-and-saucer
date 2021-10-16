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

    public Player player;    

    // Start is called before the first frame update
    void Start()
    {
        fishCollectArea.FishCollected += (fish) => OnCollectFish(fish);
    }

    void OnCollectFish(Fish fish) {
        StartedThrowFish?.Invoke();
        fish.transform.position = fishProjectilePosition.position;
        player.StartThrowFish(fish);
    }

    void StartFishing() {
        StartedFishing?.Invoke();
        player.StartFishing();
    }
}
