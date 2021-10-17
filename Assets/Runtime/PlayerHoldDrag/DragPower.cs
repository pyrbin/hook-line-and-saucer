using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DragPower : MonoBehaviour
{

    public int MaxPower;

    public float RechargeRate;
    
    public PlayerHoldDrag holdDrag;

    [HideInInspector]
    public float currentPower;

    public float PowerLeft => MaxPower - currentPower;
    
    public float PowerUsage =>  (currentPower - math.length(holdDrag.Drag));

    [HideInInspector]
    public bool active = true;

    void Awake() {
        currentPower = MaxPower;
    }

    // Start is called before the first frame update
    void Start()
    {
        holdDrag.Released += (drag) => {
            currentPower -= math.length(drag);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        holdDrag.Max = (int)currentPower;

        if (currentPower < MaxPower)
            currentPower += RechargeRate*Time.deltaTime;
    }

    public void Activate() {
        active = true;
        currentPower = MaxPower;
        holdDrag.Max = (int)currentPower;
    }

    public void Deactivate() {
        active = false;
    }
} 
