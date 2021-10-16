using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Fish))]
public class FishSwimming : MonoBehaviour, IFishStateBehaviour
{
    [NaughtyAttributes.Required]
    public Transform NosePoint;

    public LayerMask WallLayer;

    public float SwimSpeed = 1f;

    [Range(0, 60f)]
    public float Frequency = 20f;

    [Range(0, .5f)]
    public float Magnitude = 0.5f;

    public SpriteRenderer Model;

    private float3 dir = math.right();

    private float3 pos;

    bool shouldSwim = false;

    private Transform hook;

    void Awake()
    {
        ExitState();
    }

    void Update()
    {
        if (hook) transform.position = hook.position;
        if (!shouldSwim) return;

        CheckSwitch();
        Move();
    }

    public void Switch()
    {
        dir *= -1;
        Model.transform.localScale = new float3(
            Model.transform.localScale.x * -1,
            Model.transform.localScale.y,
            Model.transform.localScale.z);
    }

    void CheckSwitch()
    {
        var hit = Physics2D.Raycast(new float2(NosePoint.position.x, transform.position.y), dir.xy, 0.15f, WallLayer);
        if (hit.collider != null)
        {
            Switch();
        }
    }

    void Move()
    {
        pos += dir * Time.deltaTime * SwimSpeed;
        Model.transform.localPosition = (math.up() * math.sin(Time.time * Frequency) * Magnitude);
        transform.position = pos; 
    }

    public void SetupState()
    {
        shouldSwim = true;
        pos = transform.position;
        transform.rotation = quaternion.identity;
    }

    public void ExitState()
    {
        shouldSwim = false;
        Model.transform.rotation = quaternion.identity;
    }

    public void Catch(Transform hook)
    {
        gameObject.ForEachComponentInChildren<Collider2D>(x => x.enabled = false);
        this.hook = hook;
        shouldSwim = false;
    }

    public void RemoveHook() {
        hook = null;
    }
}
