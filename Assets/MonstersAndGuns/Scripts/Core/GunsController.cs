using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsController : MonoBehaviour
{
    [SerializeField] private int firesPerSecond = 20;
    [SerializeField] private GameObject[] guns; // en ppio son 2 guns

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
        SetStateGuns(false);
        fireRate = 1f / firesPerSecond;
        nextFire = 0f;
    }

    private IEnumerator BattleRoutine()
    {
        print("A disparar!");
        SetStateGuns(true);

        int gunToFireIndex = 0;
        while (true)
        {
            if (InputARController.IsTapping() && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                gunToFireIndex = (gunToFireIndex + 1) % guns.Length;

                // TODO: Crear la bala visual, sería un sistema de partículas
                GameManager.Instance.GunFired(gunToFireIndex);
            }

            yield return null;
            
        }
    }

    void SetStateGuns(bool state)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].SetActive(state);
        }
    }
}
