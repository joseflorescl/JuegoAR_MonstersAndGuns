using UnityEngine;

public class BulletFactory : MonoBehaviour
{
    public enum FireMethod { AllWeaponsAtOnce, OneWeaponInOrder }

    [SerializeField] private FireMethod fireMethod = FireMethod.OneWeaponInOrder;

    IWeaponController[] weapons;

    int weaponToFireIndex = 0;


    private void Awake()
    {
        weapons = GetComponentsInChildren<IWeaponController>();
    }

    public int Fire()
    {
        int min, max;
        CalculateMinMaxRange(out min, out max);

        for (int i = min; i < max; i++)
            weapons[i].Fire();

        return min;
    }

    public int FireToTarget(Vector3 target)
    {
        int min, max;
        CalculateMinMaxRange(out min, out max);

        for (int i = min; i < max; i++)
            weapons[i].FireToTarget(target);

        return min;
    }

    void CalculateMinMaxRange(out int min, out int max)
    {
        switch (fireMethod)
        {
            case FireMethod.AllWeaponsAtOnce:
                min = 0;
                max = weapons.Length;
                break;
            case FireMethod.OneWeaponInOrder:
                min = weaponToFireIndex;
                max = weaponToFireIndex + 1;
                weaponToFireIndex = (weaponToFireIndex + 1) % weapons.Length;
                break;           
            default:
                min = max = -1;
                break;
        }
    }

}
