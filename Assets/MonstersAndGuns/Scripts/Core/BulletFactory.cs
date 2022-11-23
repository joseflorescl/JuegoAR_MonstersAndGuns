using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tiene un array de weapons que puede usar una shooter (player, monster)
public class BulletFactory : MonoBehaviour
{
    public enum FireMethod { AllWeaponsAtOnce, OneWeaponInOrder } // por ahora solo se implementará que se dispare de a 1 arma a la vez: OneWeaponInOrder

    [SerializeField] private FireMethod fireMethod = FireMethod.OneWeaponInOrder;
    [SerializeField] private WeaponController[] weapons;


    int weaponToFireIndex = 0;

    public void Fire()
    {
        int min, max;
        CalculateMinMaxRange(out min, out max);

        for (int i = min; i < max; i++)
        {
            weapons[i].Fire();
        }
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
                min = max = 0;
                break;
        }
    }

}
