using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public DragPower dragPower;
    public Image MaxPower;
    public Image PowerUsage;

    // Update is called once per frame
    void Update()
    {
        MaxPower.fillAmount = dragPower.currentPower/dragPower.MaxPower;
        PowerUsage.fillAmount = dragPower.PowerUsage/dragPower.MaxPower;
    }
}
