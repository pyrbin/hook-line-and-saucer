using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FishingRod : MonoBehaviour
{

    public Bait bait;
    public Pole pole;
    PlayerHoldDrag holdDrag;


    // Start is called before the first frame update
    void Start()
    {
        bait.HitWater += () =>  {
            pole.transform.rotation = Quaternion.Euler(0,0, 60);
        };
        bait.Released += () => {
            StartCoroutine(AngleOverSeconds(pole.gameObject, 60, 0.2f));
        };
        bait.fishingRod = this;
        TryGetComponent(out holdDrag);
    }

    public IEnumerator AngleOverSeconds (GameObject objectToRotate, float end, float seconds)
    {
        float elapsedTime = 0;
        float startingAngle = objectToRotate.transform.rotation.eulerAngles.z;

        float totalAngle = math.abs(end) + math.abs(startingAngle-360);
        
        while (elapsedTime < seconds)
        {
            var angle = Mathf.SmoothStep(0, totalAngle, (elapsedTime / seconds));
            objectToRotate.transform.rotation = Quaternion.Euler(0,0, startingAngle + angle);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!bait.inWater && !bait.released) {
            pole.setRotation(math.length(holdDrag.Drag));
        }
    }
}
