using System;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
public class House : MonoBehaviour
{
    public ushort Health;

    [NaughtyAttributes.Required]
    public SpriteRenderer Model;

    public SpriteMask Mask;

#if UNITY_EDITOR
    [NaughtyAttributes.Button("Test Damage 1")]
    public void TestDamage()
    {
        Damage(1);
    }

    [NaughtyAttributes.Button("Reset")]
    public void Reset()
    {
        ValidateData();
    }
#endif

    public event Action<ushort> OnDamage;

    public event Action OnDeath;

    [NaughtyAttributes.ShowNonSerializedField()]
    private ushort currentHealth = 0;

    [NaughtyAttributes.ShowNonSerializedField()]
    private ushort maxHealth = 0;

    private float HealthFactor => ((float)currentHealth / (float)maxHealth);

    void OnValidate()
    {
        ValidateData();
    }

    void Start()
    {
#if !UNITY_EDITOR
        ValidateData();
#endif
    }

    void ValidateData()
    {
        Mask = GetComponentInChildren<SpriteMask>();
        maxHealth = Health;
        currentHealth = Health;
        Model.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        Mask.sprite = Model.sprite;
        UpdateSpritePosition();
    }

    void UpdateSpritePosition()
    {
        var diff = (1f - HealthFactor);
        var yoffset = ((Model.sprite.rect.height * 0.9f) / Model.sprite.pixelsPerUnit) * diff;

        Model.transform.localPosition = new float3(Model.transform.localPosition.x, -yoffset, Model.transform.localPosition.z);
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
        UpdateSpritePosition();

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
}
