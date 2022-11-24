using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 1;
    float health;

    public float Health => health;

    public bool IsDead => health <= 0f;

    public float CurrentHealthPercentage => IsDead ? 0f : health / maxHealth;


    [SerializeField] private GameObject vfxDeadPrefab;
    [SerializeField] private GameObject vfxDamagePrefab;



    private void Awake()
    {
        health = maxHealth;
    }


    public void Damage(float damage)
    {
        if (IsDead) return;

        health -= damage;

        if (health <= 0)
            Dead();
        else
            ReceiveDamage();

    }



    void Dead()
    {
        GameManager.Instance.DeadNotification(gameObject);

        if (vfxDeadPrefab)
            Instantiate(vfxDeadPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


    void ReceiveDamage()
    {
        if (vfxDamagePrefab)
            Instantiate(vfxDamagePrefab, transform.position, Quaternion.identity);
        //TODO: GameManager.Instance.DamageNotification(this);
    }
}
