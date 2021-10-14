using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public static class PhysicsCasts
{
    private const int MaxHitCount = 24;
    private static readonly RaycastHit[] Hits = new RaycastHit[MaxHitCount];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CapsuleCastAll(float3 top, float3 bottom, float radius, in float3 sweep,
        LayerMask collisionMask, RaycastHit[] hits, out int count)
    {
        var distance = math.length(sweep);

        count = Physics.CapsuleCastNonAlloc(top, bottom, radius, math.normalizesafe(sweep),
            Hits, distance, collisionMask, QueryTriggerInteraction.Ignore);

        return count > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CapsuleCastAll(CapsuleCollider capsule, in float3 sweep, LayerMask collisionMask, RaycastHit[] hits, out int count)
    {
        return CapsuleCastAll(capsule.bounds.center, capsule.bounds.center, capsule.radius, sweep, collisionMask, hits, out count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CapsuleCastAll(CapsuleCollider capsule, in float3 pos, in quaternion rot, in float3 sweep,
        LayerMask collisionMask, RaycastHit[] hits, out int count)
    {
        var (top, bottom, _) = capsule.CapsulePoints(pos, rot);

        return CapsuleCastAll(top, bottom, capsule.radius, sweep, collisionMask, hits, out count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CapsuleCast(float3 top, float3 bottom, float radius, in float3 sweep,
        LayerMask collisionMask, out RaycastHit hit, Collider ignoredCollider = null)
    {
        hit = default;

        return CapsuleCastAll(top, bottom, radius, sweep, collisionMask, Hits, out var count)
               && GetClosestHit(count, ignoredCollider, out hit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CapsuleCast(CapsuleCollider capsule, in float3 pos, in quaternion rot, in float3 sweep,
        LayerMask collisionMask, out RaycastHit hit, Collider ignoredCollider = null)
    {
        hit = default;

        return CapsuleCastAll(capsule, pos, rot, sweep, collisionMask, Hits, out var count)
               && GetClosestHit(count, ignoredCollider, out hit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float3 RepairHitSurfaceNormal(RaycastHit hit)
    {
        var p = hit.point + (hit.normal * 0.01f);
        var ray = new Ray(p, -hit.normal);

        return hit.collider.Raycast(ray, out var repaired, 0.015f)
            ? repaired.normal
            : hit.normal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GetClosestHit(int hitCount, Collider ignoreCollider, out RaycastHit hit)
    {
        var nearest  = -1;
        var smallest = float.MaxValue;

        for (var i = 0; i < hitCount; i++)
        {
            ref var current = ref Hits[i];

            if (current.collider == ignoreCollider) continue;
            if (current.distance <= 0f) continue;
            if (!(current.distance < smallest)) continue;

            smallest = current.distance;
            nearest = i;
        }

        if (nearest < 0)
        {
            hit = default;
            return false;
        }

        hit = Hits[nearest];
        return true;
    }
}

