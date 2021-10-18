using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fish))]
abstract public class FishSpellBehaviour : MonoBehaviour
{
    public string SpellName = "spell_name";

    public event Action Casted;

    protected Fish Caster;

    public bool IsCasting => startedCast;

    protected bool startedCast = false;

    [HideInInspector]
    public bool Available = true;

    public FMODUnity.EventReference SoundEffect;

    private void Awake()
    {
        Caster = GetComponent<Fish>();
    }

    abstract protected void OnCastStart(Fish caster);

    abstract protected void OnCastEnded(Fish caster);

    abstract protected void OnInterrupt();

    public void CastStart()
    {
        startedCast = true;
        OnCastStart(GetComponent<Fish>());
    }

    public void CastEnd()
    {
        if (!startedCast) return;

        startedCast = false;
        OnCastEnded(GetComponent<Fish>());
    }

    public void Interrupt()
    {
        if (!startedCast) return;

        startedCast = false;
        OnInterrupt();
    }

    private static List<Collider2D> results = new List<Collider2D>();

    protected IEnumerable<Ufo> UfosOverlapping(Collider2D area)
    {
        var count = Physics2D.OverlapCollider(area, new ContactFilter2D
        {
            layerMask = LayerMask.NameToLayer("Ufo")
        }, results);

        for (var i = 0; i < count; i++)
        {
            if (!results[i].TryGetComponent<Ufo>(out var ufo))
                continue;

            yield return ufo;
        }
    }

    protected void StopCast()
    {
        CastEnd();
    }

    protected void DisableControls()
    {
        Player.instance.holdDrag.Disable();
    }

    protected void EnableControls()
    {
        Player.instance.holdDrag.Enable();
    }
}
