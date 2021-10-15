using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHoldDrag : MonoBehaviour
{
    public bool Debugging = true;

    public event Action<float2> Released;

    public float2 Drag => DragPoints.Origin - DragPoints.Current;

    public (float2 Origin, float2 Current) DragPoints;

    public (float3 Origin, float3 Current) DragPointsInWorld
        => (CamToWorldPos(DragPoints.Origin), CamToWorldPos(DragPoints.Current));

    private bool recording;

    public void OnKeyPress(InputAction.CallbackContext context)
    {
        if (context.action.triggered && context.action.ReadValue<float>() != 0 &&
            context.action.phase == InputActionPhase.Performed)
        {
            TriggerPressed();
        } else if (context.action.triggered && context.action.ReadValue<float>() == default &&
            context.action.phase == InputActionPhase.Performed)
        {
            TriggerReleased();
        }
    }

    void Awake()
    {
        DragPoints.Origin = float2.zero;
        DragPoints.Current = float2.zero;
    }

    void TriggerPressed()
    {
        recording = true;
        DragPoints.Origin = Mouse.current.position.ReadValue();
        DragPoints.Current = DragPoints.Origin;
    }

    void TriggerReleased()
    {
        recording = false;

        Released?.Invoke(Drag);

        DragPoints.Origin = float2.zero;
        DragPoints.Current = float2.zero;
    }

    float3 CamToWorldPos(float2 pos)
    {
        var camPos = Camera.main.ScreenToWorldPoint(new float3(pos, 0));
        return new float3(camPos.x, camPos.y, 0);
    }

    void Update()
    {
        if (!recording) return;

        DragPoints.Current = Mouse.current.position.ReadValue();

        if (Debugging)
        {
            DebugDraw.Line(DragPointsInWorld.Origin, DragPointsInWorld.Current, Color.green);
        }

    }
}
