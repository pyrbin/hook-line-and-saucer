using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    PhysicsEvents2D physicsEvents;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out physicsEvents);
        physicsEvents.TriggerEnter += (collision) => {
            collision.gameObject.TryGetComponent<Bait>(out var bait);
            if (bait) {
                Debug.Log("Entered " + bait);
            }
        };  

        physicsEvents.TriggerExit += (collision) => {
            collision.gameObject.TryGetComponent<Bait>(out var bait);
            if (bait) {
                Debug.Log("Entered " + bait);
            }
        };  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
