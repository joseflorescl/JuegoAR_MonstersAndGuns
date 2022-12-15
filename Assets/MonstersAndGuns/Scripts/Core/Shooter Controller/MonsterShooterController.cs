using UnityEngine;

public class MonsterShooterController : BaseShooterController
{
    public virtual void FireToTarget(Vector3 target)
    {     
        bulletFactory.FireToTarget(target);
        GameManager.Instance.MonsterFired();
    }
    
}
