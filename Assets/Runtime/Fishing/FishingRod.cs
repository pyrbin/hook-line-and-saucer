using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRod : MonoBehaviour
{

    public Bait bait;
    public Pole pole;
    public PlayerHoldDrag holdDrag;
    public Transform poleTip;
    public bool locked = false;

    public int dragMax = 400;

    void Awake() {

    }

    void Start()
    {
        TryGetComponent(out holdDrag);

        holdDrag.StartDrag += () =>
        {
            if (!locked)
            {
                //pole.PoleSprite.flipX = true;
            }
        };

        holdDrag.Released += (drag) => {
            if (!locked) {
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
        bait.BeginFishing();
    }

    // Update is called once per frame
    void Update()
    {
        if (locked) return;
        
        bait.SetForce(math.length(holdDrag.Drag));

        if (!bait.inWater && holdDrag.IsDragging) {
            pole.setRotation(math.length(holdDrag.Drag));
        }

        if(holdDrag.IsDragging) {
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
        bait.shouldReel = true;
    }

    void TriggerReleased() {
        bait.shouldReel = false;
    }

}
