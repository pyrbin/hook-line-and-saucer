using ElRaccoone.Timers;
using Unity.Mathematics;
using UnityEngine;

public class EggDrop : FishSpellBehaviour
{
    public float dropRate = 0.5f;
    public ushort Ammo = 8;

    public Transform EggDropPoint;
    public GameObject ProjectilePrefab;

    private bool isDropping;
    private float elapsedTime;
    private int drops;

    private void Drop()
    {
        Instantiate(ProjectilePrefab, EggDropPoint.position, quaternion.identity);

        drops++;

        if (drops >= Ammo)
        {
            isDropping = false;
        }
    }

    private void Awake()
    {

    }

    protected override void OnCastStart(Fish caster)
    {
        Available = false;

        DisableControls();

        isDropping = true;
    }

    protected override void OnCastEnded(Fish caster)
    {
        isDropping = false;

        EnableControls();
    }

    protected override void OnInterrupt()
    {
        EnableControls();
    }

    private void Update()
    {
        if (!isDropping) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= dropRate)
        {
            elapsedTime = 0;
            Drop();
        }
    }
}
