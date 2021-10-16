using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour
{

    private LineRenderer lineRenderer;


    public Transform bait;
    public Transform pole;


    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out lineRenderer);
        lineRenderer.positionCount = 2;
    }
    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, pole.position);
        lineRenderer.SetPosition(1, bait.position);
    }
}
