using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [SerializeField] private float damage = 1f;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damage, DamageMode.Collision);
        }
    }

}
