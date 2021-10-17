using ElRaccoone.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class InkExplosionSpell : FishSpellBehaviour
{
    [Header("Damage")]
    public ushort Damage = 1;

    public float InkTime = 2;

    [Header("Visuals")]
    public Sprite InkSprite;
    public Animator InkEffects;

    [NaughtyAttributes.Required]
    public CircleCollider2D Area;

    [Space]
    public bool Debugging;

    public float ExplosionRadius => Area.radius * Area.transform.lossyScale.x;

    private void Explode()
    {
        InkEffects.SetTrigger("Bomb");

        foreach (var ufo in UfosOverlapping(Area))
        {
            ufo.Health.Damage(Damage);

            if (!ufo.Health.Empty)
            {
                ufo.Sleeping = true;
                ufo.SetEffect(InkSprite, (int)InkTime * 1000);

                Timers.SetTimeout((int)(InkTime * 1000), () =>
                {
                    ufo.Sleeping = false;
                });
            }
        }
    }

    protected override void OnCastStart(Fish caster)
    {
        Available = false;
        DisableControls();
    }

    protected override void OnCastEnded(Fish caster)
    {
        Explode();
        EnableControls();
    }

    protected override void OnInterrupt()
    {
        EnableControls();
    }

    private void Update()
    {
        if (Debugging)
        {
            DebugDraw.Circle(transform.position, ExplosionRadius, Color.red);
        }
    }
}

