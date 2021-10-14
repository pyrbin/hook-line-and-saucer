using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

using CharacterCapsule = MenteBacata.ScivoloCharacterController.CharacterCapsule;
using CharacterMover = MenteBacata.ScivoloCharacterController.CharacterMover;

/// <summary>
/// A character body for kinematic movement. Uses <see cref="MenteBacata.ScivoloCharacterController"/> internally.
/// </summary>
public class CharacterBody : MonoBehaviour
{
    [Tooltip("The capsule shape of the internal collider.")]
    [InspectorName("Capsule Shape")]
    public CapsuleShape Capsule = new()
    {
        Height = 2.0f,
        Radius = 0.5f,
        Offset = 0.0f
    };

    [Tooltip("The slope limit (in degrees). Limits the collider to only climb slopes that are less steep than the given value.")]
    [Range(25f, 75f)]
    public float SlopeLimit = 45f;

    [Tooltip("Body step height. The character will step up a stair only if it is closer to the ground than the given value.")]
    [Range(.0f, .5f)]
    public float StepOffset = 0.3f;

    [Tooltip("Small distance padding from the collider surface within which contacts are detected.")]
    [Range(0.01f, 0.05f)]
    public float SkinWidth = 0.01f;

    [Tooltip("LayerMask used when checking if a character is grounded.")]
    public LayerMask CollisionFilter = -1;

    [Tooltip("Gravity force that will be applied each call to move.")]
    public float3 Gravity = new(0f, -35f, 0f);

    /// <summary>
    /// Called on every contact hit found during 'Move'.
    /// </summary>
    public event System.Action<ContactHit> OnContactHit;

    public GroundHit GroundHit { get; private set; }

    public bool IsGrounded => GroundHit.Stable;

    public float3 Up => CharacterCapsule?.UpDirection ?? float3.zero;

    public float3 Linear { get; private set; } = float3.zero;

    public float3 Translation
    {
        get => Rigidbody.position;
        set => Rigidbody.MovePosition(value);
    }

    public quaternion Rotation
    {
        get => Rigidbody.rotation;
        set => Rigidbody.MoveRotation(value);
    }

    public Rigidbody Rigidbody { get; private set; }

    public CapsuleCollider Collider { get; private set; }

    #region External

    private CharacterCapsule CharacterCapsule;

    private CharacterMover Mover;

    private List<MenteBacata.ScivoloCharacterController.MoveContact> MoveContacts = new(10);

    #endregion

    private float3 motion;

    public void Move(float3 motion)
    {
        this.motion += motion;
    }

    private void LateFixedUpdate()
    {
        ValidateData();

        var pos = Translation;

        motion *= Time.deltaTime;
        motion += (Gravity * Time.deltaTime);

        GroundDetection(pos);

        if (GroundHit is { Stable: true })
        {
            Mover.isInWalkMode = true;

            ProjectOnGround(ref motion);
            ApplyMovingPlatform(ref motion, GroundHit.Collider);
        }
        else
        {
            Mover.isInWalkMode = false;
        }

        var original = pos;

        Mover.Move(motion, MoveContacts);

        for (var i = 0; i < MoveContacts.Count; i++)
        {
            OnContactHit?.Invoke(new()
            {
                Position = MoveContacts[i].position,
                Normal = MoveContacts[i].normal,
                Collider = MoveContacts[i].collider,
            });
        }

        Linear = (original - Translation);
        motion = float3.zero;
    }

    #region Platforms

    private void ApplyMovingPlatform(ref float3 motion, Collider collider)
    {
        if (!collider || !collider.attachedRigidbody) return;

        motion += (float3)collider.attachedRigidbody.GetPointVelocity(GroundHit.Point);
    }

    #endregion

    #region Collision

    private bool CapsuleSweep(in float3 pos, float3 sweep, out RaycastHit hit, out float3 displacement)
    {
        const float backstep = 0.1f;
        var skin = SkinWidth + backstep;

        var (len, dir) = mathx.lendir(sweep);
        var (top, bottom, _) = Collider.CapsulePoints(pos, Rotation);

        var result = PhysicsCasts.CapsuleCast(top, bottom, Collider.radius, sweep + (dir * skin), CollisionFilter, out hit);

        if (result)
        {
            hit.distance = math.max(0f, hit.distance - skin);
            displacement = dir * hit.distance;
        }
        else
        {
            displacement = dir * (len - skin);
        }


        return result;
    }

    #endregion

    #region Grounding

    private void GroundDetection(in float3 pos)
    {
        GroundHit = GroundSweep(pos);
    }

    private GroundHit GroundSweep(in float3 pos)
    {
        const float sweepLength = 0.1f;
        var sweepDirection = -Up;

        if (!CapsuleSweep(pos, sweepDirection * sweepLength, out var hit, out var _))
        {
            return default;
        }

        return new()
        {
            Angle = mathx.angle(hit.normal, Up),
            Distance = hit.distance,
            Point = hit.point,
            Normal = hit.normal,
            Collider = hit.collider,
            Stable = IsStableOnNormal(hit.normal)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsStableOnNormal(in float3 normal)
    {
        return mathx.angle(Up, normal) <= SlopeLimit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProjectOnGround(ref float3 motion)
    {
        motion = mathx.ProjectOnPlane(motion, GroundHit.Normal, Up);
    }

    #endregion

    #region Initialization

    private void OnValidate()
    {
        InitializeExternal();
        ValidateData();
    }

    private void OnEnable()
    {
        StartCoroutine(RunLateFixedUpdate());
    }

    private void OnDisable()
    {
        StopCoroutine(RunLateFixedUpdate());
    }

    private void Start()
    {
#if !UNITY_EDITOR
        InitializeExternal();
        ValidateData();
#endif
        InitializeCapsule();
        InitializeRigidbody();
    }

    private IEnumerator RunLateFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (Application.isPlaying)
                LateFixedUpdate();
        }
    }

    private void ValidateData()
    {
        Mover.maxFloorAngle = SlopeLimit;
        Mover.maxStepHeight = StepOffset;
        Mover.contactOffset = SkinWidth;
        Mover.canClimbSteepSlope = false;

        CharacterCapsule.Radius = Capsule.Radius;
        CharacterCapsule.Height = Capsule.Height;
        CharacterCapsule.VerticalOffset = Capsule.Offset;
    }

    private void InitializeExternal()
    {
        Mover = gameObject.GetOrAddComponent<CharacterMover>();
        CharacterCapsule = gameObject.GetOrAddComponent<CharacterCapsule>();
#if UNITY_EDITOR
        Mover.hideFlags = HideFlags.HideInInspector;
        CharacterCapsule.hideFlags = HideFlags.HideInInspector;
#endif
    }

    private void InitializeCapsule()
    {
        Collider = gameObject.GetComponent<CapsuleCollider>();
        Collider.direction = 1;
#if UNITY_EDITOR
        Collider.hideFlags = HideFlags.NotEditable;
#endif
    }

    private void InitializeRigidbody()
    {
        Rigidbody = gameObject.GetComponent<Rigidbody>();
        Rigidbody.isKinematic = true;
        Rigidbody.centerOfMass = float3.zero;
        Rigidbody.freezeRotation = true;
        Rigidbody.useGravity = false;
        Rigidbody.drag = 0f;
        Rigidbody.mass = 1f;
        Rigidbody.angularDrag = 0f;
        Rigidbody.centerOfMass = float3.zero;
        Rigidbody.maxAngularVelocity = math.INFINITY;
        Rigidbody.maxDepenetrationVelocity = math.INFINITY;
#if UNITY_EDITOR
        Rigidbody.hideFlags = HideFlags.NotEditable;
#endif
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (CharacterCapsule)
            GizmosDraw.WireCapsule(CharacterCapsule.LowerHemisphereCenter, CharacterCapsule.UpperHemisphereCenter, CharacterCapsule.Radius);
    }
#endif

    #endregion

}

[System.Serializable]
public struct CapsuleShape
{
    public float Offset;
    public float Height;
    public float Radius;
}

[System.Serializable]
public struct GroundHit
{
    public float Distance;
    public float Angle;
    public float3 Normal;
    public Collider Collider;
    public float3 Point;
    public bool Stable;

    public Rigidbody AttachedRigidbody => Collider?.attachedRigidbody;
}

[System.Serializable]
public struct ContactHit
{
    public float3 Normal;
    public Collider Collider;
    public float3 Position;
}
