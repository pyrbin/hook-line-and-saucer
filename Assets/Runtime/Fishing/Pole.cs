using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class Pole : MonoBehaviour
{

    public float dragToRotationModifier = 0.3f;
    public float minRotation = 0;
    public float maxRotation = 120;

    public void setRotation(float angle) {
        var a = math.clamp(angle*dragToRotationModifier, minRotation, maxRotation);
        gameObject.transform.rotation = Quaternion.Euler(0,0, -a);
    }

    public IEnumerator AngleOverSeconds (float end, float seconds)
    {
        float elapsedTime = 0;
        float startingAngle = transform.rotation.eulerAngles.z;

        float totalAngle = math.abs(end) + math.abs(startingAngle-360);
        
        while (elapsedTime < seconds)
        {
            var angle = Mathf.SmoothStep(0, totalAngle, (elapsedTime / seconds));
            transform.rotation = Quaternion.Euler(0,0, startingAngle + angle);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

}
