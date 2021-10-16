using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FishingRod : MonoBehaviour
{

    public Bait bait;
    public Pole pole;
    public PlayerHoldDrag holdDrag;
    public Transform poleTip;
    public bool locked = false;

    public int dragMax = 400;

    void Awake() {
    }

    // Start is called before the first frame update
    void Start()
    {
        holdDrag.StartDrag += () => {
            if (!locked)
                bait.StartDrag();
        };

        holdDrag.Released += (drag) => {
            if (!locked) {
                bait.ReleaseDrag();
                if (!bait.inWater || !bait.released) StartCoroutine(pole.AngleOverSeconds(60, 0.2f));
            }
        };

        bait.HitWater += () =>  {
            pole.transform.rotation = Quaternion.Euler(0,0, 60);
        };

 
        TryGetComponent(out holdDrag);
    }

    public void Reset() {
        pole.setRotation(0);
        bait.Reset();
    }



    // Update is called once per frame
    void Update()
    {
        bait.SetForce(math.length(holdDrag.Drag));

        if (!locked && !bait.inWater && !bait.released) {
            pole.setRotation(math.length(holdDrag.Drag));
        }

        if(!bait.released && bait.isDragging) {
            bait.transform.position = new Vector3(poleTip.position.x, transform.position.y, 0);
        }
    }
}
