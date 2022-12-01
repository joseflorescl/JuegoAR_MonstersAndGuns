using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterController : BaseMonsterController
{

    private void Start()
    {
        CurrentState = MonsterState.GoUp;
    }

    protected override void Attack()
    {
        //TODO
    }

    protected override void GoUp()
    {
        StartCoroutine(BossGoUpCoroutine());
    }

    protected override void Idle()
    {
        //TODO
    }

    protected override void Patrol()
    {
        StartCoroutine(BossPatrolRoutine());
    }


    IEnumerator BossGoUpCoroutine()
    {
        kinematicVelocity = GetRandomVectorUp(monsterData.maxDeviationRandomVectorUp) * monsterData.goUpSpeed;
        FaceInitialDirection();

        float secondsGoUp = Random.Range(monsterData.minSecondsGoUp, monsterData.maxSecondsGoUp);
        yield return new WaitForSeconds(secondsGoUp);
        CurrentState = MonsterState.Patrol;
    }

    IEnumerator BossPatrolRoutine()
    {
        coll.enabled = true;
        var r = monsterData.spherePatrollingRadius;
        var h = monsterData.spherePatrollingHeight;
        var d = monsterData.spherePatrollingDistanceToTarget;        

        while (CurrentState == MonsterState.Patrol)
        {
            var targetPosition = GetRandomPositionInsideSphere(GameManager.Instance.Player, r, h, d);
            var direction = targetPosition - transform.position;
            kinematicVelocity = direction.normalized * monsterData.patrolSpeed;

            // Ahora se espera: hasta llegar a este punto o haya pasado un tiempo máximo
            float secondsSameDirection = Random.Range(monsterData.minSecondsSameDirection, monsterData.maxSecondsSameDirection);
            float maxTimeInSameDirection = Time.time + secondsSameDirection;

            yield return new WaitUntil(() => Time.time > maxTimeInSameDirection || Vector3.Distance(transform.position, targetPosition) < monsterData.minDistanceToTarget);

            //TODO: pasar random a estado Attack
        }


    }
}
