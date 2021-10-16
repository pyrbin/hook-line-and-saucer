using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public DragPower dragPower;
    public Image MaxPower;

    // Update is called once per frame
    void Update()
    {
        MaxPower.fillAmount = dragPower.PowerUsage / dragPower.MaxPower;
    }
}
