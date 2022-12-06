using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(HealthController))]
public class PlayerHealthController : MonoBehaviour
{
    Collider coll;
    HealthController healthController;

    private void Awake()
    {
        coll = GetComponent<Collider>();
        healthController = GetComponent<HealthController>();
        
    }

    private void Start()
    {
        RestartHandler();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnBattling += BattleHandler;
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;
        GameManager.Instance.OnRestart += RestartHandler;
        GameManager.Instance.OnNextLevel += NextLevelHandler;

    }
    

    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= BattleHandler;
        GameManager.Instance.OnPlayerDead -= PlayerDeadHandler;
        GameManager.Instance.OnRestart -= RestartHandler;
        GameManager.Instance.OnNextLevel -= NextLevelHandler;
    }

    private void NextLevelHandler(int obj)
    {
        healthController.RestoreHealth();
    }

    private void RestartHandler()
    {
        coll.enabled = false;
        healthController.RestoreHealth();
    }

    private void PlayerDeadHandler()
    {
        coll.enabled = false;
    }

    private void BattleHandler(List<MonsterController> arg1, int arg2)
    {
        coll.enabled = true;
    }

}
