using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using NaughtyAttributes;
using System;
using UnityEngine.InputSystem;

public class Bait : MonoBehaviour
{

    public event Action Landed;

    public float forceModifier = 1;
    public float timeToThrow = 10;
    
    private float force;
    private bool released = false;
    private bool inWater = false;

    public float reelForce = 10f;
    public float2 reelDirection = new float2(0.2f, 1f);

    PlayerHoldDrag holdDrag;
    Rigidbody2D body;

    Vector3 startPoint;
    float2 fixedEndPoint;

    public bool shouldReel = false;



    void Start() {
        TryGetComponent(out holdDrag);
        TryGetComponent(out body);
        startPoint = transform.position;
        body.gravityScale = 0f;
        holdDrag.Released += (drag) => { if (!inWater && !released) Release(); };
    }

    // Update is called once per frame
    void Update()
    {
        force = math.length(holdDrag.Drag)*forceModifier;
        if (!released) {
            float2 endPoint = EndPoint();
            var direction = new float2(-force, force);
            DebugDraw.Line(transform.position, (float3)transform.position + new float3(direction.x, direction.y, 0), Color.green);
            DebugDraw.Circle(new Vector3(endPoint.x, endPoint.y, 0), new Vector3(0,0,1), 0.4f, Color.red);
        } else if (inWater){
            if (shouldReel) {
                Reel();
            }
            
        }
    }

    public void Reset() {
        transform.position = startPoint;
        inWater = false;
        released = false;
        body.gravityScale = 0f;
        body.velocity = Vector2.zero;
    }
 
    public void OnKeyPress(InputAction.CallbackContext context)
    {
        if (context.action.triggered && context.action.ReadValue<float>() != 0 &&
            context.action.phase == InputActionPhase.Performed)
        {
            TriggerPressed();
        } else if (context.action.triggered && context.action.ReadValue<float>() == default &&
            context.action.phase == InputActionPhase.Performed)
        {
            released = true;
            TriggerReleased();
        }
    }

    public void TriggerPressed() {
        shouldReel = true;
    }

    public void TriggerReleased() {
        shouldReel = false;
    }

    public void Reel() {
        body.AddForce(reelDirection*reelForce);
    }

    void Release() {
        released = true;
        fixedEndPoint = EndPoint();
        StartCoroutine(MoveOverSeconds(gameObject, new Vector3(fixedEndPoint.x, fixedEndPoint.y, 0), timeToThrow));
    }
    
    float2 EndPoint() {
        return new float2(-force + transform.position.x, transform.position.y);
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
        Landed?.Invoke();
        inWater = true;
        body.gravityScale = 0.5f;
        objectToMove.transform.position = end;
    }

}
