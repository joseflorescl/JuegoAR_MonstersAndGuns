using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnBattle += OnBattleHandler;
    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnBattle -= OnBattleHandler;
    }



    private void OnBattleHandler(List<MonsterController> enemies, int currentLevel)
    {
        StartCoroutine(OnBattleRoutine(enemies, currentLevel));
    }

    IEnumerator OnBattleRoutine(List<MonsterController> enemies, int currentLevel)
    {
        print("OnBattle: Cantidad de enemigos = " + enemies.Count + " - level = " + currentLevel);

        float secondsToAttack = 5 / currentLevel; // TODO: esto se podría controlar por una curva de animación y agregar algo de random
        yield return new WaitForSeconds(secondsToAttack);

        while (enemies.Count > 0)
        {
            // Busca al primer enemigo en estado Patrol y lo pasa a estado de Attack
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy.CurrentState == MonsterController.MonsterStates.Patrol)
                {
                    enemy.Attack();
                    break;
                }
            }

            yield return new WaitForSeconds(secondsToAttack);
        }

        print("Fin de OnBattleRoutine: ya no quedan enemigos en la lista");
        
    }

}
