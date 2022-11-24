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
        // Aqu� se llamaria al GM
        // TODO
        // GameManager.Instance.ShooterFired(this); // y el GM puede discriminar por tag qu� tipo de objeto realiz� el dispardo
        // TODO: Crear la bala visual, ser�a un sistema de part�culas
        GameManager.Instance.GunFired(gunIndex);
    }


    //TODO: este m�todo lo usar�an los monsters para dispararle al player
    // public virtual void FireBulletToTarget()
    // por esto tambi�n se necesitar�a un m�todo: TargetPosition()

    public virtual void DoDamage(GameObject opponent)
    {
        IDamageable damageable = opponent.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Damage(damage);
    }

}
