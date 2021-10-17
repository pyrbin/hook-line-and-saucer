using System;
using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour
{
    public ushort Value = 3;

    public ushort Current => currentHealth;
    public float Factor => ((float)currentHealth / (float)maxHealth);
    public bool Empty => Current <= 0;

#if UNITY_EDITOR
    [NaughtyAttributes.Button("Test Damage 1")]
    public void TestDamage()
    {
        Damage(1);
    }

    [NaughtyAttributes.Button("Reset values")]
    public void ResetValues()
    {
        ValidateData();
    }
#endif

    [NaughtyAttributes.ShowNonSerializedField()]
    private ushort currentHealth = 0;

    [NaughtyAttributes.ShowNonSerializedField()]
    private ushort maxHealth = 0 ;

    public event Action<ushort> OnDamage;

    public event Action OnDeath;

    public void Reset()
    {
        maxHealth = Value;
        currentHealth = Value;
    }

    public void Damage(ushort value)
    {
        var clamped = (ushort)math.min(value, (int)currentHealth);
        currentHealth -= clamped;

        if (clamped > 0)
            OnDamageInternal(clamped);
    }

    private void OnDamageInternal(ushort value)
    {
        OnDamage?.Invoke(value);

        if (currentHealth == 0)
        {
            OnDeathInternal();
        }
    }

    private void OnDeathInternal()
    {
        OnDeath?.Invoke();
    }

    void OnValidate()
    {
        ValidateData();
    }

    void Awake()
    {
#if !UNITY_EDITOR
        ValidateData();
#endif
    }

    void ValidateData()
    {
        Reset();
    }
}

