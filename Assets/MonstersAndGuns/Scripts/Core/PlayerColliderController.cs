using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerColliderController : MonoBehaviour
{
    Collider coll;

    private void Awake()
    {
        coll = GetComponent<Collider>();
        coll.enabled = false;
    }

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
        coll.enabled = false;
    }

    private void BattleHandler(List<MonsterController> arg1, int arg2)
    {
        coll.enabled = true;
    }

}
