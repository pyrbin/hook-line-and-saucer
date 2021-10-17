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

    [HideInInspector]
    public Fish Parent;

    public bool doDamage = false;

    void Awake()
    {
        if (Body == null)
            ValidateData();

        Parent.PhysicsEvents.TriggerEnter += (col) =>
        {
            if (col.gameObject.tag == "FishDeath")
            {
                ScheduleDespawn();
            }
            if (col.gameObject.tag == "FishStop")
            {
                Body.velocity = float2.zero;
            }
        };

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

    public void ScheduleDespawn(int timeInsMs = 300)
    {
        if (!IsActive) return;

        Body.velocity = float2.zero;
        Body.isKinematic = true;

        gameObject.ForEachComponentInChildren<Collider2D>(x => x.enabled = false);

        Timers.SetTimeout(timeInsMs, () =>
        {
            Parent.Despawn();
        });
    }

    public void Stop()
    {
        Body.velocity = float2.zero;
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

        gameObject.ForEachComponentInChildren<Collider2D>(x => x.enabled = true);

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
