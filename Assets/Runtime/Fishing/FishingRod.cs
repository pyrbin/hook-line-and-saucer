using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRod : MonoBehaviour
{


    public event Action<float2> RodReleased;

    public event Action RodStartDrag;

    public Bait bait;
    public Pole pole;
    public PlayerHoldDrag holdDrag;
    public Transform poleTip;
    public bool locked = false;

    public int dragMax = 400;

    public FMODUnity.EventReference ThrowEvent;

    void Awake() {

    }

    void Start()
    {
        TryGetComponent(out holdDrag);

        holdDrag.StartDrag += () =>
        {
            if (!locked)
            {
                    RodStartDrag?.Invoke();
                //pole.PoleSprite.flipX = true;
            }
        };

        holdDrag.Released += (drag) => {
            if (!locked) {

                FMODUnity.RuntimeManager.PlayOneShot(ThrowEvent, transform.position);
                RodReleased?.Invoke(drag);
                locked = true;
                //pole.PoleSprite.flipX = false;
                holdDrag.Disable();   
                bait.ReleaseDrag();
                if (!bait.inWater)
                    StartCoroutine(pole.AngleOverSeconds(60, 0.2f));
            }
        };

        bait.HitWater += () =>  {
            pole.transform.rotation = Quaternion.Euler(0,0, 60);
        };

        bait.ReelEnded += () =>
        {
            Reset();
            locked = false;
            holdDrag.Enable();
        };
    }

    public void Reset() {
        pole.setRotation(0);
        bait.SetupFishing();
    }

    // Update is called once per frame
    void Update()
    {
        if (locked) return;
        
        if(holdDrag.IsDragging) {
            pole.setRotation(math.length(holdDrag.Drag));
            bait.transform.position = new Vector3(poleTip.position.x, bait.transform.position.y, 0);
        }
    }

    public void OnKeyPress(InputAction.CallbackContext context)
    {
        if (context.action.triggered && context.action.ReadValue<float>() != 0 &&
            context.action.phase == InputActionPhase.Performed)
        {
            TriggerPressed();
        } else if (context.action.triggered && context.action.ReadValue<float>() == default &&
            context.action.phase == InputActionPhase.Performed)
        {
            TriggerReleased();
        }
    }
    
    void TriggerPressed() {
        if (bait.inWater) bait.reelingSoundState.start();
        bait.shouldReel = true;
    }

    void TriggerReleased() {
        bait.reelingSoundState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        bait.shouldReel = false;
    }

}
