using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FishingBar : MonoBehaviour
{
    public PlayerHoldDrag holdDrag;
    public Image Indicator;
    
    public Transform Min;
    public Transform Max;


    // Update is called once per frame
    void Update()
    {
        if (holdDrag.Max == 0) return;
        var factor = math.length(holdDrag.Drag) / holdDrag.Max;
        var yPosition = Min.position.y + factor*(Max.position.y - Min.position.y);
        Indicator.rectTransform.position = new Vector3(Indicator.rectTransform.position.x, yPosition, 0);
    }

}
