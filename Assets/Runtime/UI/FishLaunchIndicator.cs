using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FishLaunchIndicator : MonoBehaviour
{
    [NaughtyAttributes.Required]
    public PlayerThrowFish PlayerThrowFish;

    private LineRenderer LineRenderer;

    public bool display = false;

    void Start()
    {
    
        LineRenderer = GetComponentInChildren<LineRenderer>();
        LineRenderer.Reset();
        /*
     PlayerThrowFish.FirstThrow += () =>
     {
         display = false;
         LineRenderer.Reset();
     };

     GameManager.instance.StartedThrowFish += () =>
     {
         Display();
     };
     */
    }

    void Display()
    {
        /*
        LineRenderer.positionCount = 2;
        display = true;
        */
    }

    void Update()
    {
        /*
        if (!display || !PlayerThrowFish || !PlayerThrowFish.Fish) return;

        var force = new float3(PlayerThrowFish.Fish.CalculateForce(new float3(PlayerThrowFish.Force, 0)), 0);

        var (len, dir) = mathx.lendir(force);

        var clampedValue = dir * math.min(len, 5f);

        LineRenderer.SetPosition(0, PlayerThrowFish.Fish.transform.position);
        LineRenderer.SetPosition(1, (float3)PlayerThrowFish.Fish.transform.position + clampedValue);
        */
    }
}
