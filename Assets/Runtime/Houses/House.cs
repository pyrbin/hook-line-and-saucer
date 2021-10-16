using System;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
public class House : MonoBehaviour
{
    [NaughtyAttributes.Required]
    public SpriteRenderer Model;

    public SpriteMask Mask;

#if UNITY_EDITOR
    [NaughtyAttributes.Button("Test Damage 1")]
    public void TestDamage()
    {
        Health.Damage(1);
    }

    [NaughtyAttributes.Button("Reset")]
    public void Reset()
    {
        ValidateData();
    }
#endif

    [HideInInspector]
    public Health Health;

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
        TryGetComponent(out Health);

        Mask = GetComponentInChildren<SpriteMask>();

        Model.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        Mask.sprite = Model.sprite;

        UpdateSpritePosition();

        Health.OnDamage += (_) =>
        {
            UpdateSpritePosition();
        };
    }

    void UpdateSpritePosition()
    {
        var diff = 1f - Health.Factor;
        var yoffset = ((Model.sprite.rect.height * 0.9f) / Model.sprite.pixelsPerUnit) * diff;

        Model.transform.localPosition = new float3(Model.transform.localPosition.x, -yoffset, Model.transform.localPosition.z);
    }
}
