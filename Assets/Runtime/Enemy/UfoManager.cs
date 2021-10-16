
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
        var point = SpawnArea.GetRandomPointInsideCollider();
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
}

