using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esta clase se encarga de disparar balas, por lo que puede ser usada por el player y por los monsters
public abstract  class ShooterController : MonoBehaviour
{
    protected BulletFactory bulletFactory;


    private void Awake()
    {
        bulletFactory = GetComponent<BulletFactory>();
    }


    public virtual void FireBullet()
    {
        bulletFactory.Fire();
        // Aqu� se llamaria al GM
        // TODO
        // GameManager.Instance.ShooterFired(this); // y el GM puede discriminar por tag qu� tipo de objeto realiz� el dispardo
    }


    //TODO: este m�todo lo usar�an los monsters para dispararle al player
    // public virtual void FireBulletToTarget()
    // por esto tambi�n se necesitar�a un m�todo: TargetPosition()

}
