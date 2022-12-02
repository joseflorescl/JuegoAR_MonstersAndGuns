using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWeaponController : MonoBehaviour, IWeaponController
{
    [SerializeField] private MissileController missilePrefab;
    [SerializeField] private Transform FirePoint;

    public void Fire() // Por ahora no se está usando pero se implementa igual
    {
        Instantiate(missilePrefab, FirePoint.position, FirePoint.rotation);
    }

    public void FireToTarget(Vector3 target)
    {
        var direction = target - FirePoint.position;
        var rotation = Quaternion.LookRotation(direction);
        Instantiate(missilePrefab, FirePoint.position, rotation);
    }
}
