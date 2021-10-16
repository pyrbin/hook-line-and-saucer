
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class UfoManager : MonoBehaviour
{
    public GameObject UfoPrefab;

    public int MaxUfoObjects = 3;
    public float SpawnRate = 2;

    public int TotalUfos => Ufos.Count;

    public static UfoManager instance;

    public void Awake()
    {
        if (!instance)
        {
            instance = this;

        } else {
            DestroyImmediate(this.gameObject);
        }
    }

    [SerializeField]
    public List<House> Targets= new();

    private List<Ufo> Ufos = new();

    public BoxCollider2D SpawnArea;

    public void Start()
    {
        StartCoroutine(nameof(SpawnInterval));
    }

    public void Spawn()
    {
        var point = GetRandomPointInsideCollider(SpawnArea);
        var ufo = Instantiate(UfoPrefab, point, quaternion.identity, transform);

        Ufos.Add(ufo.GetComponent<Ufo>());
    }

    public void RemoveSelf(Ufo ufo)
    {
        try
        {
            Ufos.RemoveAt(Ufos.FindIndex(x => x == ufo));
        } catch
        {
            //ignored
        }
    }

    public House GetNewTarget()
    {
        return Targets.Where(x => Ufos.All(u => u.Target != x)).FirstOrDefault() ??
               Targets.ElementAt(UnityEngine.Random.Range(0, Targets.Count-1));
    }

    IEnumerator SpawnInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnRate);

            if (MaxUfoObjects > TotalUfos)
                Spawn();
        }
    }

    float3 GetRandomPointInsideCollider(BoxCollider2D boxCollider)
    {
        var extents = boxCollider.size / 2f;
        var point = new float2(UnityEngine.Random.Range(-extents.x, extents.x), UnityEngine.Random.Range(-extents.y, extents.y)) + (float2)boxCollider.offset;

        return ((float3)boxCollider.transform.TransformPoint(new Vector3(point.x, point.y, 0)));
    }
}

