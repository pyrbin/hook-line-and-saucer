using System;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
public class House : MonoBehaviour
{
    [NaughtyAttributes.Required]
    public SpriteRenderer Model;

    public SpriteMask Mask;

    [HideInInspector]
    public Health Health;

    public FMODUnity.EventReference onDestroySound;
    
    public FMODUnity.EventReference onHitSound;

    public int OnHitSoundPercentage = 40;

#if UNITY_EDITOR
    [NaughtyAttributes.Button("Validate Data")]
    public void ValidateDataButton()
    {
        ValidateData();
    }
#endif

    void OnValidate()
    {
        ValidateData();
    }

    void Start()
    {
#if !UNITY_EDITOR
        ValidateData();
#endif
        Health.OnDeath += () => {
            FMODUnity.RuntimeManager.PlayOneShot(onDestroySound, transform.position);
        };

        Health.OnDamage += (_) => {
            if (OnHitSoundPercentage > UnityEngine.Random.Range(0, 100))
                FMODUnity.RuntimeManager.PlayOneShot(onHitSound, transform.position);
        };

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
