
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public List<GameObject> Prefabs;

    public int MaxFishes = 5;
    public float2 SpawnRateRange = new(1.53f,3.23f);

    public int TotalFishes => Fishes.Count;

    public static FishSpawner instance;

    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
    }

    public List<BoxCollider2D> SpawnAreas;

    private List<Fish> Fishes = new();

    public void Start()
    {
        StartCoroutine(nameof(SpawnInterval));
    }

    public void Spawn()
    {
        var point = SpawnAreas.ElementAt(UnityEngine.Random.Range(0, SpawnAreas.Count )).GetRandomPointInsideCollider();
        var fish = Instantiate(Prefabs.ElementAt(UnityEngine.Random.Range(0, Prefabs.Count)), point, quaternion.identity, transform);

        Fishes.Add(fish.GetComponent<Fish>());
    }

    public void RemoveSelf(Fish fish)
    {
        try
        {
            Fishes.RemoveAt(Fishes.FindIndex(x => x == fish));
        }
        catch
        {
            // ignored
        }
    }

    IEnumerator SpawnInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(SpawnRateRange.x, SpawnRateRange.y));

            if (MaxFishes > TotalFishes)
                Spawn();
        }
    }
}

