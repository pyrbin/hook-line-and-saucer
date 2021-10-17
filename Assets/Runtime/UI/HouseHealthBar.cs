using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image health;

    // Update is called once per frame
    void Update()
    {
        health.fillAmount = playerHealth.factor;
    }
}
