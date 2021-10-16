using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerThrowFish : MonoBehaviour
{

    public float Dampening = 0.933f;

    public bool Debugging = false;


    [Space]
    public PlayerHoldDrag HoldDrag;

    public FishProjectile Fish;

    public int dragMax = 50;

    public float2 Force => HoldDrag.Drag * (1f - Dampening);

    void Start()
    {
        HoldDrag.Released += (_) =>
        {
            if (Fish)
                Fish.ApplyForce(new float3(Force, 0));
        };
    }

    void Update()
    {
        if (HoldDrag.IsDragging && Fish)
        {
            var offset = new float3(Fish.CalculateForce(new float3(Force, 0)), 0);

            if (Debugging)
                Debug.DrawLine(Fish.transform.position, (float3)Fish.transform.position + offset, Color.green);
        }
    }
}
