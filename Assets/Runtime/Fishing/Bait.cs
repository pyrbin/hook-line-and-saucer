using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using NaughtyAttributes;
using System;
using UnityEngine.InputSystem;

public class Bait : MonoBehaviour
{

    public event Action HitWater;
    public event Action Released;

    public float forceModifier = 1;
    public float timeToThrow = 10;
    

    [SerializeField]
    private float force;
    public bool released = false;
    public bool inWater = false;

    public float reelForce = 10f;
    public float2 reelDirection = new float2(0.2f, 1f);

    public PlayerHoldDrag holdDrag;
    public Rigidbody2D body;

    Vector3 startPoint;
    float2 fixedEndPoint;

    public bool shouldReel = false;
    private bool startDrag = false;

    [Range(-20, 0)]
    public float maxDistance = 10;
    [Range(-20, 0)]
    public float minDistance = 3;
    [Range(-20, 0)]
    public float distanceOffset;

    void OnDrawGizmos() {
        if (!inWater && !released) {
            DebugDraw.Circle(new Vector3(maxDistance + distanceOffset , 0, 0) + transform.position, new Vector3(0,0,1), 0.4f, Color.red);
            DebugDraw.Circle(new Vector3(minDistance + distanceOffset, 0, 0) + transform.position, new Vector3(0,0,1), 0.4f, Color.black);
            DebugDraw.Circle(new Vector3(distanceOffset, 0, 0) + transform.position, new Vector3(0,0,1), 0.4f, Color.yellow);
        }
    }

    void Start() {
        TryGetComponent(out body);
        startPoint = transform.position;
        body.gravityScale = 0f;
        holdDrag.StartDrag += () => { if (!inWater && !released) startDrag = true; };
        holdDrag.Released += (drag) => { if (!inWater && !released && startDrag) Release(); startDrag = false; };
    }

    // Update is called once per frame
    void Update()
    {
        force = math.length(holdDrag.Drag)*forceModifier;
        if (!released && startDrag) {
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
        body.angularVelocity = 0f;
        body.rotation = 0f;
    }
 
    public void Reel() {
        body.AddForce(reelDirection*reelForce);
    }

    void Release() {
        released = true;
        fixedEndPoint = EndPoint();
        Released?.Invoke();
        StartCoroutine(MoveOverSeconds(gameObject, new Vector3(fixedEndPoint.x, fixedEndPoint.y, 0), timeToThrow));
    }
    
    float2 EndPoint() {

        var x = math.clamp((-force), maxDistance, minDistance);
        return new float2(x + distanceOffset + transform.position.x, transform.position.y);
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
            TriggerReleased();
        }
    }

    public void TriggerPressed() {
        shouldReel = true;
    }

    public void TriggerReleased() {
        shouldReel = false;
    }
    
    public void setInWater() {
        HitWater?.Invoke();
        inWater = true;
    }

    public IEnumerator MoveOverSeconds (GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            var x = Mathf.SmoothStep(startingPos.x, end.x, (elapsedTime / seconds));
            var y = Mathf.Sin(Mathf.SmoothStep(0, Mathf.PI, (elapsedTime / seconds)))*8;
            objectToMove.transform.position = new Vector3(x, startingPos.y + y, 0);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        body.gravityScale = 0.5f;
    }

}
