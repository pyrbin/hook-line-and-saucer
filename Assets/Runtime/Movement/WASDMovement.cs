using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CharacterBody))]
public class WASDMovement : MonoBehaviour
{
    public float Speed;

    [HideInInspector]
    public float2 Move;

    public CharacterBody Body { get; private set; }

    private void FixedUpdate()
    {
        var input = (float3)(transform.right * Move.x + transform.forward * Move.y);
        var motion = input * Speed;

        Body.Move(motion);
    }

    private void Awake() => ValidateData();

    private void OnValidate() => ValidateData();

    private void ValidateData()
    {
        Body = gameObject.GetOrAddComponent<CharacterBody>();
    }
}
