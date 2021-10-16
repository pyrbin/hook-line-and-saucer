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
    public float throwHeight = 2;
    

    [SerializeField]
    private float force;
    public bool released = false;
    public bool inWater = false;

    public float reelForce = 10f;
    public float2 reelDirection = new float2(0.2f, 1f);

    public PlayerHoldDrag holdDrag;
    public Rigidbody2D body;

    public Transform poleTip;

    [HideInInspector]
    public PhysicsEvents2D physicsEvents;

    Vector3 startPoint;
    float2 fixedEndPoint;

    public FishingRod fishingRod;

    public bool shouldReel = false;
    private bool startDrag = false;

    [Range(-20, 0)]
    public float maxDistance = 10;
    [Range(-20, 0)]
    public float minDistance = 3;
    [Range(-20, 0)]
    public float distanceOffset;


    private Fish catchedFish;

    void Awake() {
        startPoint = transform.position;
    }

    void OnDrawGizmos() {
        if (!inWater && !released && math.length(holdDrag.Drag) <= 0) {
            DebugDraw.Circle(new Vector3(maxDistance + distanceOffset , 0, 0) + transform.position, new Vector3(0,0,1), 0.2f, Color.red);
            DebugDraw.Circle(new Vector3(minDistance + distanceOffset, 0, 0) + transform.position, new Vector3(0,0,1), 0.2f, Color.black);
            DebugDraw.Circle(new Vector3(distanceOffset, 0, 0) + transform.position, new Vector3(0,0,1), 0.2f, Color.yellow);
        }
    }

    void Start() {
        TryGetComponent<PhysicsEvents2D>(out physicsEvents);

        physicsEvents.CollisionEnter += (collider) => {
            if (collider.gameObject.TryGetComponent<FishSwimming>(out var fish)) {
                if (!catchedFish) {
                    fish.Catch(transform);
                    catchedFish = fish.GetComponent<Fish>();
                }
            }
        };

        physicsEvents.TriggerEnter += (collision) => {
            if (collision.TryGetComponent<Water>(out var Water))  {
                setInWater();
                reelDirection = new float2(0.1f, 1f);
            }

            if (collision.TryGetComponent<FishCollectArea>(out var fishCollect)) {
                if (inWater) Reset();
                if (catchedFish && fishingRod)  {
                    fishCollect.CollectFish(catchedFish, fishingRod);
                    catchedFish = null;
                }
            }
        };  

        physicsEvents.TriggerExit += (collision) => {
            if (collision.TryGetComponent<Water>(out var Water))  {
                reelDirection = new float2(0.1f, 0);
                body.velocity =  new Vector2(body.velocity.x, 0);
            }
        };  

        TryGetComponent(out body);
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
            DebugDraw.Circle(new Vector3(endPoint.x, endPoint.y, 0), new Vector3(0,0,1), 0.2f, Color.red);
            transform.position = new Vector3(poleTip.position.x, transform.position.y, 0);
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
        shouldReel = false;
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
        return new float2(x + distanceOffset + startPoint.x, startPoint.y);
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
            var y = Mathf.Sin(Mathf.SmoothStep(0, Mathf.PI, (elapsedTime / seconds)))*throwHeight;
            objectToMove.transform.position = new Vector3(x, startingPos.y + y, 0);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        body.gravityScale = 0.5f;
    }

}
