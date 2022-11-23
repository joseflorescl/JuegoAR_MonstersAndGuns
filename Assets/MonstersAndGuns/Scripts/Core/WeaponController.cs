using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script se le asocia a cada una de las armas que tengrá el player (ó monster).
//  y provee un método para disparar
public class WeaponController : MonoBehaviour
{
    ParticleSystem vfxBullet;

    private void Awake()
    {
        vfxBullet = GetComponentInChildren<ParticleSystem>();
    }
    public void Fire()
    {
        vfxBullet.Play();
        // TODO: animar el recoil del weapon
    }
  
}
