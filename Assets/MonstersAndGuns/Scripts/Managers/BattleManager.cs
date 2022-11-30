using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private float secondsToAttackLevel1 = 10f;

    private void OnEnable()
    {
        GameManager.Instance.OnBattling += BattleHandler;
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= BattleHandler;
        GameManager.Instance.OnPlayerDead -= PlayerDeadHandler;
    }

    private void PlayerDeadHandler()
    {
        StopAllCoroutines();
    }


    private void BattleHandler(List<MonsterController> monsters, int currentLevel)
    {
        StartCoroutine(BattleRoutine(monsters, currentLevel));
    }

    IEnumerator BattleRoutine(List<MonsterController> monsters, int currentLevel)
    {
        var secondsToAttack = secondsToAttackLevel1 / currentLevel; // TODO: esto se podr�a controlar por una curva de animaci�n y agregar algo de random

        while (monsters.Count > 0)
        {
            yield return new WaitForSeconds(secondsToAttack);
            // Busca al primer monstruo en estado Patrol y lo pasa a estado de Attack
            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];
                if (monster.CurrentState == MonsterState.Patrol)
                {
                    monster.Attack();
                    break;
                }
            }
        }

        // TODO: aqu� se podr�a mandar una se�al al GM:
        //  GameManager.Instance.BattleEnded()
        
    }

}
