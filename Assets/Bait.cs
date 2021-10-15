using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using NaughtyAttributes;

public class Bait : MonoBehaviour
{

    [Range(0.0f, 100.0f)]
    public float force = 1;

    float2 direction;
    bool released = false;
    float2 fixedEndPoint;

    public float distanceModifier = 10;


    // Update is called once per frame
    void Update()
    {
        if (!released) {
            direction = new float2(-force, force);
            float2 endPoint = EndPoint();
            DebugDraw.Line(transform.position, (float3)transform.position + new float3(direction.x, direction.y, 0), Color.green);
            DebugDraw.Circle(new Vector3(endPoint.x, endPoint.y, 0), new Vector3(0,0,1), 0.4f, Color.red);
            if (Input.GetKeyDown(KeyCode.Space)) {
                Release();
            }
        }
    }

    public IEnumerator MoveOverSeconds (GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            var x = Mathf.Lerp(startingPos.x, end.x, (elapsedTime / seconds));
            var y = Mathf.Sin(Mathf.Lerp(0, Mathf.PI, (elapsedTime / seconds)))*4;
            objectToMove.transform.position = new Vector3(x, y, 0);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }

    void Release() {
        released = true;
        fixedEndPoint = EndPoint();
        StartCoroutine(MoveOverSeconds(gameObject, new Vector3(fixedEndPoint.x, fixedEndPoint.y, 0), 1));
    }
    
    float2 EndPoint() {
        return new float2(direction.x * distanceModifier + transform.position.x, transform.position.y);
    }


}
