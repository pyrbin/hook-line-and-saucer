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
    public event Action ReelEnded;

    public bool Debugging = true;

    [Range(-20, 0)]
    public float maxDistance = 10;
    [Range(-20, 0)]
    public float minDistance = 3;
    [Range(-20, 0)]
    public float distanceOffset;

    public float forceModifier = 1;

    public float timeToThrow = 1;

    public float throwHeight = 4;

    public float reelForce = 2f;

    public float2 reelDirection = new float2(0.1f, 1f);

    private float2 currentReelDirection;

    [HideInInspector]
    public bool inWater = false;

    [HideInInspector]
    public bool shouldReel = false;

    private float force = 0;

    private Vector3 startPoint;

    public Fish fishOnHook;

    private Rigidbody2D body;

    private PhysicsEvents2D physicsEvents;

    private FishCollectArea fishCollectArea;

    private float2 EndPoint => new float2(math.clamp((-force), maxDistance, minDistance) + distanceOffset + startPoint.x, startPoint.y);

    private void OnValidate()
    {
        startPoint = transform.position;
    }

    void Awake() {
        startPoint = transform.position;
        currentReelDirection = reelDirection;
        TryGetComponent(out physicsEvents);
        TryGetComponent(out body);
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("BaitCatched"));
    }

    public void SetupFishing()
    {
        transform.position = startPoint;
        inWater = false;
        body.gravityScale = 0f;
        body.velocity = Vector2.zero;
        body.angularVelocity = 0f;
        body.rotation = 0f;
        shouldReel = false;
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("BaitCatched"));
    }


    public void CatchFish(FishSwimming fish) {
        Debug.Log("Catched!");
        fish.Catch(transform);
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("BaitCatched"));
        fishOnHook = fish.GetComponent<Fish>();
    }



    private void CollectFish() {
        if (fishCollectArea == null) return;
        SetupFishing();
        fishCollectArea.CollectFish(fishOnHook);
        fishOnHook = null;
    }

    void Start() {
        physicsEvents.CollisionEnter += (collider) => {

            if (fishOnHook == null &&
                collider.gameObject.TryGetComponent<Fish>(out var parent) && parent.FishState == FishState.Swimming &&
                collider.gameObject.TryGetComponent<FishSwimming>(out var fish))
            {   
                CatchFish(fish);
                if (fishCollectArea != null)
                    CollectFish();
            }
        };

        physicsEvents.TriggerEnter += (collision) => {
            if (collision.TryGetComponent<Water>(out var Water))  {
                setInWater();
                currentReelDirection = reelDirection;
            }

            if (collision.TryGetComponent<FishCollectArea>(out var fishCollectArea)) {
                this.fishCollectArea = fishCollectArea;
                if (fishOnHook)
                {
                    CollectFish();
                }
                else if (inWater)
                {
                    ReelEnded?.Invoke();
                    SetupFishing();
                }
            }
        };  

        physicsEvents.TriggerExit += (collision) => {
            if (collision.TryGetComponent<Water>(out var Water))  {
                currentReelDirection = new float2(0.4f, 0);
                body.velocity =  new Vector2(body.velocity.x, 0);
            }

            if (collision.TryGetComponent<FishCollectArea>(out var _)) {
                fishCollectArea = null;
            }
        };  

        body.gravityScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Debugging) {
            float2 endPoint = EndPoint;
            DebugDraw.Circle(new Vector3(endPoint.x, endPoint.y, 0), new Vector3(0,0,1), 0.2f, Color.red);
        }   
       
        if (inWater && shouldReel) {
            Reel();
        }
    }

    void OnDrawGizmos() {
        if (!inWater && Debugging) {
            DebugDraw.Circle(new Vector3(maxDistance + distanceOffset , 0, 0) + startPoint, new Vector3(0,0,1), 0.2f, Color.red);
            DebugDraw.Circle(new Vector3(minDistance + distanceOffset, 0, 0) + startPoint, new Vector3(0,0,1), 0.2f, Color.black);
            DebugDraw.Circle(new Vector3(distanceOffset, 0, 0) + startPoint, new Vector3(0,0,1), 0.2f, Color.yellow);
        }
    }

    public void ReleaseDrag() {
        Release();
    }


    public void SetForce(float force) {
        this.force = force*forceModifier;
    }

    public void Reel() {
        body.AddForce(currentReelDirection*reelForce);
    }

    void Release() {
        var endPoint = EndPoint;
        StartCoroutine(MoveOverSeconds(gameObject, new Vector3(endPoint.x, endPoint.y, 0), timeToThrow));
    }
    
    public void setInWater() {
        if (!inWater) {
            //First time hitting the water
            HitWater?.Invoke();
            gameObject.SetLayerRecursively(LayerMask.NameToLayer("Bait"));
        }
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
