
using ElRaccoone.Timers;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(Rigidbody2D))]
public class Ufo : MonoBehaviour
{
    [Header("Movement")]
    public float Speed = 1.0f;
    public float2 SpeedRandom = new float2(0.8f, 1.5f);
    public float XRandom = 1f;

    [Header("Range")]
    public float AttackRange = 2;
    public float2 AttackRangeRandom = new float2(0.8f, 1.5f);

    [Header("Weapon")]
    public float AttackTime;
    public GameObject ProjectilePrefab;

    public SpriteRenderer Renderer;
    public SpriteRenderer RendererEffect;

    public ParticleSystem Explosions;

    [Space]
    public bool Debugging = true;
    public bool Sleeping = false;

#if UNITY_EDITOR
    [NaughtyAttributes.Button("Test Death")]
    public void TestDeath()
    {
        Health?.Damage(Health.Value);
    }
#endif

    [NaughtyAttributes.ReadOnly]
    public House Target;

    private float attackTimer = 0;
    private float3 cacheTargetPos = float3.zero;

    [HideInInspector]
    public Health Health;

    private Rigidbody2D Body;

    private bool WaitForStabilize = true;

    public void ThrowFish(Fish fish, float2 thrust, bool doDamage)
    {
        if (doDamage)
            Health.Damage(fish.Stats.Damage);

        if (Health.Empty)
            return;

        Knockback(thrust);
    }

    public void Knockback(float2 thrust)
    {
        const float thrustMod = 0.85f;

        Body.AddForce(thrust * thrustMod, ForceMode2D.Force);

        WaitForStabilize = true;
    }

    public void SetEffect(Sprite sprite, int duration)
    {
        if (RendererEffect)
        {
            RendererEffect.enabled = true;
            RendererEffect.sprite = sprite;

            Timers.SetTimeout(duration, () =>
            {
                try
                {
                    RendererEffect.enabled = false;
                    RendererEffect.sprite = null;
                } catch
                {
                    // ignored
                }
            });
        }
    }

    void Start()
    {
        ValidateData();
        RendererEffect.enabled = false;

        Health.OnDamage += (d) =>
        {
            Renderer.color = Color.red;

            Timers.SetTimeout(1000, () => {
                if (Renderer)
                {
                    Renderer.color = Color.white;
                }
            });
        };

        Health.OnDeath += () =>
        {
            GameManager.AddScore();

            Sleeping = true;

            Explosions.Play();

            gameObject.ForEachComponentInChildren<Collider2D>(x => x.enabled = false);

            Timers.SetTimeout(800, () =>
            {
                Renderer.enabled = false;
            });

            Timers.SetTimeout(1500, () =>
            {
                Destroy(this.gameObject);
            });
        };
    }

    void ValidateData()
    {
        TryGetComponent(out Health);
        TryGetComponent(out Body);

        Body.gravityScale = 0f;
        Body.drag = 2f;

        AttackRange *= UnityEngine.Random.Range(AttackRangeRandom.x, AttackRangeRandom.y);
        Speed *= UnityEngine.Random.Range(SpeedRandom.x, SpeedRandom.y);

        Health.Reset();
    }

    void Update()
    {
        if (WaitForStabilize)
        {
            if (Body.velocity.magnitude <= .75f)
                WaitForStabilize = false;
            return;
        }

        if (Sleeping) return;

        if (UfoManager.instance == null)
        {
            Sleeping = true;
            return;
        }

        if (Target)
        {
            PursueTarget();
        }
        else
        {

            Target = UfoManager.instance.GetNewTarget();

            if (Target == null) return;

            cacheTargetPos = Target.transform.position;
            cacheTargetPos.x += UnityEngine.Random.Range(-XRandom, XRandom);

            var middle = Target.GetComponentInChildren<Collider2D>().ClosestPoint(new float2(Target.transform.position.x, 99999));
            cacheTargetPos.y = middle.y + AttackRange;

            Target.Health.OnDeath += () =>
            {
                Target = null;
            };
        }
    }

    void OnDestroy()
    {
        UfoManager.instance?.RemoveSelf(this);
    }

    void PursueTarget()
    {
        if (Target.Health.Empty)
        {
            Target = null;
            return;
        }

        if (math.distance(transform.position, cacheTargetPos) <= math.EPSILON)
        {
            Attack();
        } else {
            Move(cacheTargetPos);
        }
    }

    void Attack()
    {
        if (attackTimer <= 0)
        {
            Shoot();
        }

        attackTimer = math.max(0, attackTimer - Time.deltaTime);
    }

    void Shoot()
    {
        attackTimer = AttackTime;

        if (Debugging)
            Debug.DrawLine(transform.position, Target.transform.position, Color.red, AttackTime/2f);

        Instantiate(ProjectilePrefab, transform.position, quaternion.identity);
    }

    void Move(float3 pos)
    {
        var step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, pos, step);
        if (Debugging)
            Debug.DrawLine(transform.position, pos, Color.green);
    }
}

