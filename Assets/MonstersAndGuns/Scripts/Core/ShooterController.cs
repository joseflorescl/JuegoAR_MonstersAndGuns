using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract  class ShooterController : MonoBehaviour
{
    // Esta clase se encarga de disparar balas, por lo que puede ser usada por el player y por los monsters
    [SerializeField] protected float damage = 1f;

    protected BulletFactory bulletFactory;
    
    protected virtual void Awake()
    {
        bulletFactory = GetComponentInChildren<BulletFactory>();
    }


    public virtual void FireBullet()
    {
        int gunIndex = bulletFactory.Fire();                
        GameManager.Instance.GunFired(gunIndex); // TODO: el GM también necesitará saber qué object realizó el disparo (Player, Monster)
    }

    public virtual void DoDamage(GameObject opponent)
    {
        IDamageable damageable = opponent.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Damage(damage);
    }

    // public virtual void FireBulletToTarget() //TODO: este método lo usarían los monsters para dispararle al player
    // por esto también se necesitaría un método: TargetPosition()

}
