using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public PlayerHoldDrag holdDrag;
    public PlayerThrowFish throwFish;
    public FishingRod fishingRod;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out throwFish);
        TryGetComponent(out fishingRod);
        TryGetComponent(out holdDrag);
        StartFishing();
    }

    public void CastSpell(InputAction.CallbackContext context)
    {
        if (throwFish?.Fish?.Parent?.Spell == null) return;

        if (context.action.triggered && context.action.ReadValue<float>() != 0 &&
            context.action.phase == InputActionPhase.Performed)
        {
            throwFish?.Fish?.Parent?.Spell?.CastStart();
        }
        else if (context.action.triggered && context.action.ReadValue<float>() == default &&
          context.action.phase == InputActionPhase.Performed)
        {
            throwFish?.Fish?.Parent?.Spell?.CastEnd();
        }
    }

    public void StartThrowFish(Fish fish) {
        var fishSwimming = fish.gameObject.GetComponent<FishSwimming>();       
        fishSwimming.RemoveHook();
        fish.SetState(FishState.Projectile);

        throwFish.Fish = fish.GetComponent<FishProjectile>();
        throwFish.hasNotThrown = true;

        holdDrag.Enable();
        holdDrag.Max = throwFish.dragMax;

        fishingRod.locked = true;
    }

    public void StartFishing()
    {
        fishingRod.Reset();
        holdDrag.Enable();
        holdDrag.Max = fishingRod.dragMax;
        fishingRod.locked = false;
    }
}
