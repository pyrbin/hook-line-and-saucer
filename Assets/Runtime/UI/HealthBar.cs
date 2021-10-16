using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health Health;
    public Image Display;

    // Update is called once per frame
    void Update()
    {
        Display.fillAmount = Health.Factor;

        if (Display.fillAmount == 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
