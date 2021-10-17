using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EggProjectile : MonoBehaviour
{
    public float Speed = 10f;
    public ushort DamageAmount = 2;

    void Update()
    {
        var step = Speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down * step, step);

        var angle = Mathf.Atan2(Vector3.down.y, Vector3.down.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var ufo = other?.GetComponentInParent<Ufo>();
        if (ufo)
        {
            ufo.Health.Damage(DamageAmount);
            ufo.Knockback(Vector2.down * 3f);
            Destroy(this.gameObject);
        }
    }
}

