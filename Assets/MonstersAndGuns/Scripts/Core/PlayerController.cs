using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float waitBeforeActivateGuns = 2f;


    private void OnEnable()
    {
        GameManager.Instance.OnBattle += OnBattleHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnBattle -= OnBattleHandler;
    }

    private void OnBattleHandler(List<MonsterController> arg1, int arg2)
    {

        StartCoroutine(PlayerOnBattleRoutine());
    }

    IEnumerator PlayerOnBattleRoutine()
    {
        // Se espera un poco antes de activar las guns
        yield return new WaitForSeconds(waitBeforeActivateGuns);
        print("A disparar!");
    }

    private void OnDrawGizmos()
    {
        var color = Color.red;
        color.a = 0.1f;
        Gizmos.color = color;
        Gizmos.DrawSphere(Vector3.zero, 10);
    }
}
