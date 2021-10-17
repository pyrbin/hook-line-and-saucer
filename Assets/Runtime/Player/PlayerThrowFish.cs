using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerThrowFish : MonoBehaviour
{

    public float Dampening = 0.933f;

    public float MinForce = 0.5f;

    public bool Debugging = false;


    [Space]
    public PlayerHoldDrag HoldDrag;

    public FishProjectile Fish;

    public int dragMax = 50;

    public event Action FirstThrow;

    public float2 Force => HoldDrag.Drag * (1f - Dampening);

    [HideInInspector]
    public bool hasNotThrown = true;

    void Start()
    {
        HoldDrag.Released += (_) =>
        {
            if (Fish && math.length(HoldDrag.Drag) > MinForce)
            {
                Fish.ApplyForce(new float3(Force, 0));
                if (hasNotThrown)
                {
                    FirstThrow?.Invoke();
                    hasNotThrown = false;
                }
            }
        };
    }

    void Update()
    {
        if (HoldDrag.IsDragging && Fish)
        {
            var offset = new float3(Fish.CalculateForce(new float3(Force, 0)), 0);

            if (hasNotThrown)
            {
                Fish.RotateToForce(offset.xy);
            }

            if (Debugging)
                Debug.DrawLine(Fish.transform.position, (float3)Fish.transform.position + offset, Color.green);
        }
    }
}
