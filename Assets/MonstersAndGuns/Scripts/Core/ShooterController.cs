using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esta clase se encarga de disparar balas, por lo que puede ser usada por el player y por los monsters
public abstract  class ShooterController : MonoBehaviour
{
    [SerializeField] protected float damage = 1f;

    protected BulletFactory bulletFactory;
    


    protected virtual void Awake()
    {
        bulletFactory = GetComponentInChildren<BulletFactory>();
    }


    public virtual void FireBullet()
    {
        int gunIndex = bulletFactory.Fire();
        // Aquí se llamaria al GM
        // TODO
        // GameManager.Instance.ShooterFired(this); // y el GM puede discriminar por tag qué tipo de objeto realizó el dispardo
        // TODO: Crear la bala visual, sería un sistema de partículas
        GameManager.Instance.GunFired(gunIndex);
    }


    //TODO: este método lo usarían los monsters para dispararle al player
    // public virtual void FireBulletToTarget()
    // por esto también se necesitaría un método: TargetPosition()

    public virtual void DoDamage(GameObject opponent)
    {
        IDamageable damageable = opponent.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Damage(damage);
    }

}
