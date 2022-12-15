using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerWeaponController : MonoBehaviour, IWeaponController
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
        
        if (!anim.IsInTransition(0))
            anim.SetTrigger("Fire");
    }

    public void FireToTarget(Vector3 target) { }
}
