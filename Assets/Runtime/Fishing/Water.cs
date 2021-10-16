using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Water : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<PhysicsEvents2D>(out var physicsEvents);

        physicsEvents.TriggerEnter += (collision) => {
            collision.TryGetComponent<Bait>(out var bait);
            bait.setInWater();
            bait.reelDirection = new float2(0.1f, 1f);
        };  

        physicsEvents.TriggerExit += (collision) => {
            collision.TryGetComponent<Bait>(out var bait);
            bait.reelDirection = new float2(0.1f, 0);
            bait.body.velocity =  new Vector2(bait.body.velocity.x, 0);
        };  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
