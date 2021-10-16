using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public float Speed = 10f;
    public ushort DamageAmount = 1;

    void Update()
    {
        var step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down * step, step);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var house = other?.GetComponentInParent<House>();
        if (house)
        {
            house.Damage(DamageAmount);
            Destroy(this.gameObject);
        }
    }
}

