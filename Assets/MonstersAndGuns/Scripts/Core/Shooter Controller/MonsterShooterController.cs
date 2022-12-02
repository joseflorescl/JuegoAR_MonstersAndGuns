using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterShooterController : ShooterController
{
    public virtual void FireToTarget(Vector3 target)
    {     
        bulletFactory.FireToTarget(target);
        GameManager.Instance.MonsterFired();
    }
    
}
