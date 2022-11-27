using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactory : MonoBehaviour
{
    public enum FireMethod { AllWeaponsAtOnce, OneWeaponInOrder }

    [SerializeField] private FireMethod fireMethod = FireMethod.OneWeaponInOrder;
    
    WeaponController[] weapons; // Tiene un array de weapons que puede usar una shooter (player, monster)

    int weaponToFireIndex = 0;


    private void Awake()
    {
        weapons = GetComponentsInChildren<WeaponController>();
    }

    public int Fire()
    {
        int min, max;
        CalculateMinMaxRange(out min, out max);

        for (int i = min; i < max; i++)
        {
            weapons[i].Fire();
        }

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
