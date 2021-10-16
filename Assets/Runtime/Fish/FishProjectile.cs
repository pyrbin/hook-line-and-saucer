using System;
using ElRaccoone.Timers;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Fish), typeof(Rigidbody2D))]
public class FishProjectile : MonoBehaviour, IFishStateBehaviour
{
    public event Action<Ufo> HitUfo;

    public float RotationFactor = 15f;

    [NaughtyAttributes.ShowNativeProperty]
    public bool IsFlying => !Body.isKinematic;

    public bool IsActive = false;

#if UNITY_EDITOR
    public float2 TestForce = new float2(0f, 10f);
    [NaughtyAttributes.Button("Test Up Force")]
    public void TestApplyForce()
    {
        ApplyForce(new float3(TestForce, 0f));
    }
#endif

    Rigidbody2D Body;

    Fish Parent;

    public bool doDamage = false;

    void Awake()
    {
#if !UNITY_EDITOR
        ValidateData();
#endif
        ExitState();
    }

    public void ApplyForce(float3 drag)
    {
        if (!IsActive) return;

        Body.isKinematic = false;
        Body.AddForce(CalculateForce(drag), ForceMode2D.Impulse);
        doDamage = true;
    }

    public float2 CalculateForce(float3 drag)
    {
        var (len, dir) = mathx.lendir(drag);
        return dir.xy * len * Parent.Stats.SpeedMult;
    }

    public void ScheduleDespawn(int timeInsMs = 1000)
    {
        if (!IsActive) return;

        Body.isKinematic = true;

        foreach(var co in GetComponentsInChildren<Collider2D>())
        {
            co.enabled = false;
        }

        Timers.SetTimeout(timeInsMs, () =>
        {
            Parent.Despawn();
        });
    }

    void FixedUpdate()
    {
        if (IsFlying)
        {
            Body.MoveRotation(Body.rotation + (RotationFactor * Body.velocity.magnitude) * Time.fixedDeltaTime);
        }
    }

    void OnCollision(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Ufo ufo))
        {
            HitUfo?.Invoke(ufo);

            ufo.ThrowFish(Parent, Body.velocity * Parent.Stats.Weight, doDamage);

            doDamage = false;

            Body.velocity = float2.zero;
        }
    }

    public void SetupState()
    {
        Body.isKinematic = true;
        Body.velocity = float2.zero;
        IsActive = true;
        doDamage = false;

        Parent.PhysicsEvents.CollisionEnter += OnCollision;
    }

    public void ExitState()
    {
        Body.isKinematic = true;
        Body.velocity = float2.zero;
        IsActive = false;
        doDamage = false;

        Parent.PhysicsEvents.CollisionEnter -= OnCollision;
    }

    void OnValidate()
    {
        ValidateData();
    }

    void ValidateData()
    {
        TryGetComponent(out Body);
        TryGetComponent(out Parent);
    }
}
