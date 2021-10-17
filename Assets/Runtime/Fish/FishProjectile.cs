using System;
using ElRaccoone.Timers;
using Unity.Mathematics;
using UnityEngine;

public enum RotationMode
{
    Spinning,
    AlongForce
}

[RequireComponent(typeof(Fish), typeof(Rigidbody2D))]
public class FishProjectile : MonoBehaviour, IFishStateBehaviour
{
    public event Action<Ufo> HitUfo;

    public float RotationFactor = 15f;
    public RotationMode RotationMode = RotationMode.Spinning;

    [NaughtyAttributes.ShowNativeProperty]
    public bool IsFlying => !Body.isKinematic;

    public float2 Linear => Body.velocity;

    public float2 Direction => math.normalize(Linear);

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

    public void ApplyForce(float2 drag)
    {
        ApplyForce(new float3(drag, 0));
    }

    public float2 CalculateForce(float3 drag)
    {
        var (len, dir) = mathx.lendir(drag);
        return dir.xy * len * Parent.Stats.SpeedMult;
    }

    public void ScheduleDespawn(int timeInsMs = 300)
    {
        if (!IsActive) return;

        IsActive = false;

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

    public void RotateToForce(float2 force)
    {
        var angle = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), RotationFactor * Time.fixedDeltaTime);
    }

    void Update()
    {

        if (IsFlying)
        {
            switch (RotationMode)
            {
                case RotationMode.Spinning:
                    Body.MoveRotation(Body.rotation + (RotationFactor * Body.velocity.magnitude) * Time.fixedDeltaTime);
                    break;
                case RotationMode.AlongForce:

                    if (Body.velocity.magnitude > 0.35f)
                    {
                        RotateToForce(Body.velocity);
                    }
                    break;
            }
        }
    }

    void OnCollision(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Ufo ufo))
        {
            HitUfo?.Invoke(ufo);

            ufo.ThrowFish(Parent, Body.velocity * Parent.Stats.Weight, doDamage);

            if (ufo.Health.Empty)
            {
                GameManager.instance.dragPower.AddPower(GameManager.instance.dragPower.MaxPower * (1 / 4));
            }

            doDamage = false;

            Body.velocity *= 0.05f;
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
