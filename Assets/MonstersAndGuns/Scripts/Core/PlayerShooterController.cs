using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooterController : ShooterController
{
    //TODO: mover la actual lógica de GunsControoler a este script
    //  y el script GunsController se puede borrar

    [SerializeField] private int firesPerSecond = 20;

    float fireRate;
    float nextFire;

    private void OnEnable()
    {
        GameManager.Instance.OnBattling += BattleHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= BattleHandler;
    }

    private void BattleHandler(List<MonsterController> arg1, int arg2)
    {

        StartCoroutine(BattleRoutine());
    }

    private void Start()
    {
        bulletFactory.gameObject.SetActive(false);
        fireRate = 1f / firesPerSecond;
        nextFire = 0f;
    }


    private IEnumerator BattleRoutine()
    {
        // TODO por revisar desde aquí
        print("A disparar!");
        bulletFactory.gameObject.SetActive(true);

        int gunToFireIndex = 0;
        while (true)
        {
            if (InputARController.IsTapping() && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                gunToFireIndex = (gunToFireIndex + 1) % 2; // TODO: esto ya no iría

                FireBullet();                
            }

            yield return null;

        }
    }

   
}
