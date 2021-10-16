using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public PlayerHoldDrag holdDrag;
    public PlayerThrowFish throwFish;
    public FishingRod fishingRod;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out throwFish);
        TryGetComponent(out fishingRod);
        TryGetComponent(out holdDrag);
        StartFishing();
    }

    public void StartThrowFish(Fish fish) {

        var fishSwimming = fish.gameObject.GetComponent<FishSwimming>();
        fishSwimming.RemoveHook();
        fish.SetState(FishState.Projectile);
        
        throwFish.Fish = fish.GetComponent<FishProjectile>();
        holdDrag.Max = throwFish.dragMax;

        fishingRod.Reset();
        fishingRod.locked = true;
    }

    internal void StartFishing()
    {
        holdDrag.Max = fishingRod.dragMax;
        fishingRod.locked = false;
    }
}
