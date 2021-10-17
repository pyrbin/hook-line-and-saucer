using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public event Action OnDeath; 

    public House[] Houses;

    public float factor => 0 + Houses.Sum(x => x.Health.Factor)/Houses.Length;

    public int MaxHealth { get; private set; }

    void Update() {
        if (factor <= 0) {
            Kill();
        }
    }

    void Kill() {
        OnDeath?.Invoke();
    }

}
