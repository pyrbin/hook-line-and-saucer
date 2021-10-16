
using Unity.Mathematics;
using UnityEngine;

public class Ufo : MonoBehaviour
{
    [Header("Movement")]
    public float Speed = 1.0f;
    public float2 SpeedRandom = new float2(0.8f, 1.5f);

    [Header("Range")]
    public float AttackRange = 2;
    public float2 AttackRangeRandom = new float2(0.8f, 1.5f);

    [Header("Weapon")]
    public float AttackTime;
    public GameObject ProjectilePrefab;

    [Space]
    public bool Debugging = true;

    [NaughtyAttributes.ReadOnly]
    public House Target;

    private float attackTimer = 0;
    private bool isAttacking = false;
    private float3 cacheTargetPos = float3.zero;

    void Start()
    {
        AttackRange *= UnityEngine.Random.Range(AttackRangeRandom.x, AttackRangeRandom.y);
        Speed *= UnityEngine.Random.Range(SpeedRandom.x, SpeedRandom.y);
    }

    void Update()
    {
        if (Target)
        {
            PursueTarget();
        } else {
            Target = UfoManager.instance.GetNewTarget();

            cacheTargetPos = Target.transform.position;
            cacheTargetPos.x += UnityEngine.Random.Range(-1, 1f);

            Target.OnDeath += () =>
            {
                Target = null;
            };
        }
    }

    void OnDestroy()
    {
        UfoManager.instance.RemoveSelf(this);
    }

    void PursueTarget()
    {
        var targetPos = CalculateTargetPos();

        if (math.distance(transform.position, targetPos) <= math.EPSILON && ScanTarget())
        {
            Attack();
        } else {
            Move(targetPos);
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

    bool ScanTarget()
    {
        var hit = Physics2D.Raycast(transform.position, Vector3.down, math.INFINITY, LayerMask.GetMask("House"));
        return hit.collider != null ? hit.collider.gameObject : null == Target?.gameObject;
    }

    void AvoidanceLogic()
    {
        var hit = Physics2D.Raycast(transform.position, Vector3.down, math.INFINITY, LayerMask.GetMask("Ufo"));

        if (hit.collider != null)
        {
            var other = hit.collider?.GetComponent<Ufo>();

            if (other && other.Target == Target && math.abs(other.AttackRange - AttackRange) <= 0.5)
            {
                AttackRange = math.max(1, AttackRange + ((UnityEngine.Random.Range(0, 1f) > 0.5f) ? 1 : -1f));
            }
        }
    }

    float3 CalculateTargetPos()
    {
        AvoidanceLogic();

        var yoffset = transform.position.y;
        var origin = new float2(cacheTargetPos.x, cacheTargetPos.y + AttackRange * 2);
        var hit = Physics2D.Raycast(origin, Vector3.down, math.INFINITY, LayerMask.GetMask("House"));

        if (hit.collider?.GetComponentInParent<House>() == Target)
        {
            yoffset = math.max(hit.point.y, hit.point.y + AttackRange);

            if (Debugging)
            {
                Debug.DrawLine(transform.position, hit.collider.transform.position);
                Debug.DrawLine(hit.point, hit.point + new Vector2(0, AttackRange));
            }
        }

        var targetPos = new float3(cacheTargetPos.x, yoffset, 0f);

        return targetPos;
    }
}

