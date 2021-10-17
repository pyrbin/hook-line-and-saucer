using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fish))]
abstract public class FishSpellBehaviour : MonoBehaviour
{
    public string SpellName = "spell_name";

    public event Action Casted;

    public bool IsCasting => startedCast;

    protected bool startedCast = false;

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

    private static List<Collider2D> results = new();

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
}
