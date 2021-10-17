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
    public ParticleSystem Effects;

    [Space]
    public bool Debugging;

    private float timeCounter = 0;
    private bool charging = false;

    public float ChargeFactor => timeCounter / MaxTime;

    private float3 initialScale;
    private float3 initialEffectScale;

    public float ExplosionRadius => Area.radius * Area.transform.lossyScale.x;

    private void Update()
    {
        if (Debugging)
        {
            DebugDraw.Circle(transform.position, ExplosionRadius, Color.red);
        }

        if (charging)
        {
            timeCounter = math.min(timeCounter + Time.deltaTime, MaxTime);

            transform.localScale = initialScale + (initialScale * (ChargeFactor * MaxScale));

            Effects.transform.localScale = initialEffectScale + (initialEffectScale * (ChargeFactor * MaxScale));

            if (timeCounter >= MaxTime)
            {
                Explode();
                StopCast();
            }
        }
    }

    private void Explode()
    {
        charging = false;
        FMODUnity.RuntimeManager.PlayOneShot(SoundEffect, transform.position);

        foreach (var ufo in UfosOverlapping(Area))
        {
            ufo.Health.Damage(Damage);

            var dir = math.normalize((ufo.transform.position - transform.position));
            var force = dir * KnockbackForce;

            ufo.Knockback(force.xy);
        }

        Effects.Play();
    }

    protected override void OnCastStart(Fish caster)
    {
        Available = false;

        initialScale = transform.localScale;
        initialEffectScale = Effects.transform.localScale;

        Player.instance.holdDrag.Disable();

        caster.Projectile.Stop();

        charging = true;
    }

    protected override void OnCastEnded(Fish caster)
    {
        if (charging)
            Explode();

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

