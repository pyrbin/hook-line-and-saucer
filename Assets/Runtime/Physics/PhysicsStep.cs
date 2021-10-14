using Unity.Mathematics;
using UnityEngine;

[DefaultExecutionOrder(3000)]
public class PhysicsStep : MonoBehaviour
{
    [Header("Simulation")]
    [Tooltip("Set physics timescale")]
    public float TimeScale = 1f;

    [Tooltip("Set physics gravity")]
    public float3 Gravity = new (0f, -10f, 0f);

    public static float DeltaTime
#if UNITY_EDITOR
        => UnityEditor.EditorApplication.isPaused ? Time.fixedDeltaTime : Mathf.Max(Time.deltaTime, 1E-05f);
#else
        => Mathf.Max(Time.deltaTime, 1E-05f);
#endif

    private void OnValidate()
    {
        ValidateData();
    }

    private void Awake()
    {
        ValidateData();
    }

    private void ValidateData()
    {
        Time.timeScale = TimeScale;
        Physics.gravity = Gravity;
    }

    private void Update()
    {
        if (Physics.autoSimulation)
            return;

        if (Time.timeScale == 0f)
            return;

        Physics.Simulate(DeltaTime);
    }
}
