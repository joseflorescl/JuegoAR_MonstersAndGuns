using UnityEngine;

public class MonsterWeaponController : MonoBehaviour, IWeaponController
{
    [SerializeField] private MissileController missilePrefab;
    [SerializeField] private Transform FirePoint;

    public void Fire() // Por ahora no se está usando pero se implementa igual
    {
        var missile = PoolManager.Instance.Get(missilePrefab, FirePoint.position, FirePoint.rotation);
        missile.Init();
    }

    public void FireToTarget(Vector3 target)
    {
        var direction = target - FirePoint.position;
        var rotation = Quaternion.LookRotation(direction);
        var missile = PoolManager.Instance.Get(missilePrefab, FirePoint.position, rotation);
        missile.Init(); // Pool Manager Compatible
    }
}
