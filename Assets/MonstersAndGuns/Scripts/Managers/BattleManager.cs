using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private float secondsToAttack = 60f;
    private void OnEnable()
    {
        GameManager.Instance.OnBattling += OnBattleHandler;
    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= OnBattleHandler;
    }



    private void OnBattleHandler(List<MonsterController> monsters, int currentLevel)
    {
        StartCoroutine(OnBattleRoutine(monsters, currentLevel));
    }

    IEnumerator OnBattleRoutine(List<MonsterController> monsters, int currentLevel)
    {
        print("OnBattle: Cantidad de monstruos = " + monsters.Count + " - level = " + currentLevel);

        secondsToAttack = 20 / currentLevel; // TODO: esto se podría controlar por una curva de animación y agregar algo de random
        yield return new WaitForSeconds(secondsToAttack);

        while (monsters.Count > 0)
        {
            // Busca al primer monstruo en estado Patrol y lo pasa a estado de Attack
            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];
                if (monster.CurrentState == MonsterController.MonsterState.Patrol)
                {
                    monster.Attack();
                    break;
                }
            }

            yield return new WaitForSeconds(secondsToAttack);
        }

        print("Fin de OnBattleRoutine: ya no quedan monstruos en la lista");
        
    }

}
