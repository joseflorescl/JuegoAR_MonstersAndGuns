using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script se le asocia a cada una de las armas que tengr� el player (� monster).
//  y provee un m�todo para disparar
[RequireComponent(typeof(Animator))]
public class WeaponController : MonoBehaviour
{
    ParticleSystem vfxBullet;
    Animator anim;

    private void Awake()
    {
        vfxBullet = GetComponentInChildren<ParticleSystem>();
        anim = GetComponent<Animator>();
    }
    public void Fire()
    {
        vfxBullet.Play();

        // Si ya se est� animando, entonces no gatillaremos el trigger nuevamente
        if (!anim.IsInTransition(0))
            anim.SetTrigger("Fire");
    }

    
}
