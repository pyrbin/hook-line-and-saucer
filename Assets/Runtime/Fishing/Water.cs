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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
