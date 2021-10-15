using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBaitOnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<PhysicsEvents2D>(out var physicsEvents);
        physicsEvents.CollisionEnter += (collider) => {
            collider.gameObject.TryGetComponent<Bait>(out var bait);
            bait.Reset();
        };  

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
