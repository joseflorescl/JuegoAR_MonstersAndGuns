using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMonsterController : BaseMonsterController
{
    protected override void Attack() { }

    protected override void GoUp() { }

    protected override void Idle() { }

    protected override void Patrol() { }

    private void Start()
    {
        CurrentState = MonsterState.Idle;
    }

    private void OnDisable()
    {
        healthController.Damage(healthController.Health, DamageMode.Collision);
    }

}
