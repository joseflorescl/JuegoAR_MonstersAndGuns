using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsController : MonoBehaviour
{
    [SerializeField] private float waitBeforeActivateGuns = 2f;
    [SerializeField] private GameObject[] guns; // en ppio son 2 guns

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
    }

    private IEnumerator BattleRoutine()
    {
        // Se espera un poco antes de activar las guns
        yield return new WaitForSeconds(waitBeforeActivateGuns);
        print("A disparar!");
        SetStateGuns(true);
    }

    void SetStateGuns(bool state)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].SetActive(state);
        }
    }
}
