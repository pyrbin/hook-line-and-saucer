using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCollectArea : MonoBehaviour
{
    public Action<Fish> FishCollected;

    public void CollectFish(Fish fish) {
        if (fish != null)
            FishCollected?.Invoke(fish);
    }
}
