using ElRaccoone.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class ExplosionSpell : FishSpellBehaviour
{
    [Header("Damage")]
    public ushort Damage = 3;
    public float KnockbackForce = 3;

    [Header("Scale & Speed")]
    public float MaxTime = 1f;
    public float MaxScale = 2f;

    [NaughtyAttributes.Required]
    public CircleCollider2D Area;

    [Space]
    public bool Debugging;

    private float timeCounter = 0;
    private bool charging = false;

    public float ChargeFactor => timeCounter / MaxTime;

    private float3 initialScale;

    private void Update()
    {
        if (charging)
        {
            timeCounter = math.min(timeCounter + Time.deltaTime, MaxTime);

            transform.localScale = initialScale + (initialScale * (ChargeFactor * MaxScale));

            if (timeCounter >= MaxTime)
            {
                Explode();
                StopCast();
            }
        }

        if (Debugging)
        {
            DebugDraw.Circle(transform.localPosition, Area.radius * transform.localScale.x, Color.red);
        }
    }

    private void Explode()
    {
        charging = false;

        var results = new List<Collider2D>();

        var count = Physics2D.OverlapCollider(Area, new ContactFilter2D
        {
            layerMask = LayerMask.NameToLayer("Ufo")
        }, results);

        for (var i = 0; i < count; i++)
        {
            if (!results[i].TryGetComponent<Ufo>(out var ufo)) continue;

            ufo.Health.Damage(Damage);

            var dir = math.normalize((ufo.transform.position - transform.position));
            var force = dir * KnockbackForce;

            ufo.Knockback(force.xy);
        }
    }

    protected override void OnCastStart(Fish caster)
    {
        initialScale = transform.localScale;
        Player.instance.holdDrag.Disable();
        caster.Projectile.Stop();
        charging = true;
    }

    protected override void OnCastEnded(Fish caster)
    {
        if (charging) Explode();

        ResetControls();

        caster.HideVisuals();
        caster.Projectile.ScheduleDespawn(1300);
    }


    protected override void OnInterrupt()
    {
        ResetControls();
    }

    private void ResetControls()
    {
        Player.instance.holdDrag.Enable();
    }
}

