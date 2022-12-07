using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [SerializeField] private float damage = 1f;


    private void OnTriggerEnter(Collider other)
    {
        print("Causa Damage: " + name);

        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damage, DamageMode.Collision);
        }
    }

}
