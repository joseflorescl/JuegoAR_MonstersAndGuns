using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private float secondsToAttackLevel1 = 10f;
    [SerializeField] private float substractSecondsToAttackNewLevel = 0.5f;
    [SerializeField] private float minSecondsToAttack = 1f;

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


    private void BattleHandler(int currentLevel)
    {
        StartCoroutine(BattleRoutine(currentLevel));
    }

    IEnumerator BattleRoutine(int currentLevel)
    {
        //Esto se podría controlar por una curva de animación y agregar algo de random
        var secondsToAttack = secondsToAttackLevel1 - (currentLevel - 1) * substractSecondsToAttackNewLevel;
        secondsToAttack = Mathf.Clamp(secondsToAttack, minSecondsToAttack, secondsToAttack);

        var monsters = GameManager.Instance.Monsters;

        while (monsters.Count > 0)
        {
            yield return new WaitForSeconds(secondsToAttack);
            // Busca al primer monstruo en estado Patrol y lo pasa a estado de Attack
            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];
                if (monster.CurrentState == MonsterState.Patrol)
                {
                    monster.DoAttack();
                    break;
                }
            }
        }        
    }

}
