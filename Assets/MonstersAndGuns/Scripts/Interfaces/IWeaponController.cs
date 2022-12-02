using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponController
{
    void Fire();

    void FireToTarget(Vector3 target);

}
