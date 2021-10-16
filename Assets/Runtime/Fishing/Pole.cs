using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class Pole : MonoBehaviour
{

    public PlayerHoldDrag holdDrag;

    public float dragToRotationModifier = 0.3f;
    public float minRotation = 0;
    public float maxRotation = 120;

    public Transform poleBase;

    // Start is called before the first frame update
    void Start()
    {
        holdDrag.StartDrag += () => {  };
        holdDrag.Released += (drag) => {  };

    }

    public void setRotation(float angle) {
        var a = math.clamp(angle*dragToRotationModifier, minRotation, maxRotation);
        gameObject.transform.rotation = Quaternion.Euler(0,0, -a);
    }

}
