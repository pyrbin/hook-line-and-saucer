using ElRaccoone.Timers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public enum FishState
{
    Projectile,
    Swimming
}

public interface IFishStateBehaviour
{
    void SetupState();
    void ExitState();
}

[System.Serializable]
public struct FishStats
{
    public ushort Damage;
    public ushort SpeedMult;
    public ushort Weight;
}

[RequireComponent(typeof(FishSwimming), typeof(FishProjectile))]
public class Fish : MonoBehaviour
{
    public FishState FishState;

    [SerializeField]
    public FishStats Stats = new()
    {
        Damage = 1,
        SpeedMult = 1,
        Weight = 1  
    };

    public FishSwimming Swimming;

    public FishProjectile Projectile;

    public FishSpellBehaviour Spell;

    [HideInInspector]
    public PhysicsEvents2D PhysicsEvents;

    public event Action<FishStats> Despawned;

#if UNITY_EDITOR
    [NaughtyAttributes.Button("Toggle State")]
    public void ToggleState()
    {
        if (FishState == FishState.Projectile)
        {
            SetState(FishState.Swimming);
        } else {
            SetState(FishState.Projectile);
        }
    }
#endif

    void Start()
    {
#if !UNITY_EDITOR
        ValidateData();
#endif
        CurrentState.SetupState();
    }

    public void Despawn()
    {
        if (Spell && Spell.IsCasting)
            Spell.Interrupt();

        Despawned?.Invoke(Stats);

        Timers.SetTimeout(300, () => {
            FishSpawner.instance.RemoveSelf(this);
            Destroy(this.gameObject);
        });
    }

    public void HideVisuals()
    {
        Swimming.Model.enabled = false;
    }

    public void SetState(FishState state)
    {
        if (FishState == state) return;

        EnterState(state);
    }

    public IFishStateBehaviour CurrentState => FishState == FishState.Swimming ? Swimming : Projectile;

    void EnterState(FishState state)
    {
        CurrentState.ExitState();
        FishState = state;
        CurrentState.SetupState();
    }

    void OnValidate()
    {
        ValidateData();
    }

    void ValidateData()
    {
        PhysicsEvents = GetComponentInChildren<PhysicsEvents2D>();

        TryGetComponent(out Swimming);
        TryGetComponent(out Projectile);
    }
}
