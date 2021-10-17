using System;
using UnityEngine;

[RequireComponent(typeof(Fish))]
abstract public class FishSpellBehaviour : MonoBehaviour
{
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

    protected void StopCast()
    {
        CastEnd();
    }
}
