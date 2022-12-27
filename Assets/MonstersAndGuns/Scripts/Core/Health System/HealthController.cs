using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 1;
    float health;

    public float Health => health;
    public bool IsDead => health <= 0f;
    public float CurrentHealthPercentage => IsDead ? 0f : health / maxHealth;


    private void OnEnable() => RestoreHealth(); // Pool Manager Compatible

    public void RestoreHealth()
    {
        health = maxHealth;
    }

    public void Damage(float damage, DamageMode mode)
    {
        if (IsDead) return;

        health -= damage;

        if (health <= 0)
            Dead(mode);
        else
            ReceiveDamage(mode);
    }

    void Dead(DamageMode mode)
    {
        GameManager.Instance.DeadNotification(this, mode);
    }

    void ReceiveDamage(DamageMode mode)
    {
        GameManager.Instance.DamageNotification(this);
    }
}
