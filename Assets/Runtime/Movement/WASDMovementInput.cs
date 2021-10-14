using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(WASDMovement))]
public class WASDMovementInput : MonoBehaviour
{
    [Space]
    [NaughtyAttributes.ReadOnly]
    public WASDMovement Movement;

    public void OnMove(InputAction.CallbackContext input)
    {
        Movement.Move = input.ReadValue<Vector2>();
    }

    private void Awake() => ValidateData();

    private void OnValidate() => ValidateData();

    private void ValidateData()
    {
        TryGetComponent(out Movement);
    }
}
