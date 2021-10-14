using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public static class CapsuleColliderExtensions
{
    /// <summary>
    /// Returns top, bottom & center points in world position for collider at given position & rotation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float3 Top, float3 Bottom, float3 Center) CapsulePoints(this CapsuleCollider capsule, in float3 pos, in quaternion rot)
    {
        var center = math.transform(new RigidTransform { pos = pos, rot = rot }, capsule.center);

        var bottom = new float3(center.x, center.y - capsule.height  / 2 + capsule.radius, center.z);
        var top = new float3(center.x, center.y + capsule.height / 2 - capsule.radius, center.z);

        return (top, bottom, center);
    }

    /// <summary>
    /// Inflates the collider by given offset.
    /// </summary>
    public static void Inflate(this CapsuleCollider capsule, float offset)
    {
        capsule.radius += offset;
        capsule.height += offset * 2.0f;
    }
}
