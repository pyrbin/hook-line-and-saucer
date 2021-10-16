using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBaitOnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<PhysicsEvents2D>(out var physicsEvents);
        physicsEvents.TriggerEnter += (collider) => {
            collider.gameObject.TryGetComponent<Bait>(out var bait);
            if (bait.inWater)
                bait.Reset();
        };  

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
