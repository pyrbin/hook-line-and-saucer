using ElRaccoone.Timers;
using UnityEngine;

public class DashSpell : FishSpellBehaviour
{
    public float Force = 5f;
    public ushort Damage = 3;

    public PhysicsEvents2D Area;
    public SpriteRenderer DashEffects;

    private bool isDashing = false;

    private void Dash(Fish caster)
    {
        FMODUnity.RuntimeManager.PlayOneShot(SoundEffect, transform.position);
        caster.Projectile.ApplyForce(caster.Projectile.Direction * Force);
        DashEffects.gameObject.SetActive(true);
    }

    private void Awake()
    {
        Area.TriggerEnter += OnTrigger;
        Area.CollisionEnter += OnCollision;

        Area.gameObject.SetActive(false);
        DashEffects.gameObject.SetActive(false);
    }

    private void OnTrigger(Collider2D col)
    {
        if (!isDashing) return;

        if (col.gameObject.TryGetComponent(out Ufo ufo))
        {
            ufo.Health.Damage(Damage);
            GameManager.instance.dragPower.AddPower(GameManager.instance.dragPower.MaxPower);
        }
    }

    private void OnCollision(Collision2D col)
    {
        OnTrigger(col.collider);
    }

    protected override void OnCastStart(Fish caster)
    {
        Available = false;
        DisableControls();

        caster.Swimming.Model.color = new Color(1, 1, 1, 0.5f);

        gameObject.SetLayerRecursively(LayerMask.NameToLayer("FishDash"));
        Area.gameObject.SetActive(true);
        isDashing = true;

        Dash(caster);

        Timers.SetTimeout(1000, () =>
        {
            try
            {
                caster.Swimming.Model.color = new Color(1, 1, 1, 1f);
                EnableControls();
                isDashing = false;
                Area.gameObject.SetActive(false);
                gameObject.SetLayerRecursively(LayerMask.NameToLayer("Fish"));
                DashEffects.gameObject.SetActive(false);
            }
            catch
            {
                // ignored
            }
        });
    }

    protected override void OnCastEnded(Fish caster)
    {

    }

    protected override void OnInterrupt()
    {
        EnableControls();
    }


    private void Update()
    {

    }
}
