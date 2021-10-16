using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCollectArea : MonoBehaviour
{

    public Action<Fish, FishingRod> FishCollected;

    public void CollectFish(Fish fish, FishingRod fishingRod) {
        FishCollected?.Invoke(fish, fishingRod);
    }
}
