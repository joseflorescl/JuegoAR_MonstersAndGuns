using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script se le asocia a cada una de las armas que tengrá el player (ó monster).
//  y provee un método para disparar
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
        
        if (!anim.IsInTransition(0)) // Si ya se está animando, entonces no gatillaremos el trigger nuevamente
            anim.SetTrigger("Fire");
    }

}
