using System.Collections;
using UnityEngine;

public class MonsterController : BaseMonsterController
{
    protected override void Init()
    {
        base.Init();
        GameManager.Instance.MonsterCreated(this);

        if (monsterData.goUpSpeed == 0)
            CurrentState = MonsterState.Idle;
        else
            CurrentState = MonsterState.GoUp;
    }

    protected override void Idle() { }

    protected override void GoUp()
    {
        StartCoroutine(GoUpCoroutine());
    }

    protected override void Patrol()
    {
        StartCoroutine(PatrolRoutine());
    }

    protected override void Attack()
    {
        StartCoroutine(AttackRoutine());
    }

    public void DoAttack()
    {
        CurrentState = MonsterState.Attack; // El paso a estado Attack es controlado por el BattleManager
    }

    IEnumerator GoUpCoroutine()
    {
        coll.enabled = false; // Primero se le desactiva el collider mientras el monstruo va subiendo, y se activará al inicio del Patrol

        kinematicVelocity = GetRandomVectorUp(monsterData.maxDeviationRandomVectorUp) * monsterData.goUpSpeed;
        FaceInitialDirection();

        float secondsGoUp = Random.Range(monsterData.minSecondsGoUp, monsterData.maxSecondsGoUp);
        float maxTime = Time.time + secondsGoUp;

        yield return new WaitWhile(() => (Time.time < maxTime) 
                                      && (DistanceToPlayer > monsterData.minDistanceToPlayer));

        if (Time.time < maxTime) // Entonces el yield anterior terminó porque el monstruo está muy cerca del player
        {
            var direction = GetDirectionAwayFromPlayer();
            kinematicVelocity = direction * monsterData.goUpSpeed;
            FaceInitialDirection();
            yield return new WaitForSeconds(maxTime - Time.time);
        }

        CurrentState = MonsterState.Patrol;
    }


    Vector3 GetDirectionAwayFromPlayer()
    {
        var direction = transform.position - GameManager.Instance.PlayerPosition;
        direction.y = 0;
        var angle = Random.Range(-monsterData.angleToAwayFromPlayer, +monsterData.angleToAwayFromPlayer);
        direction = Quaternion.Euler(0, angle, 0) * direction;
        direction.Normalize();
        return direction;
    }

    

    IEnumerator PatrolRoutine()
    {
        coll.enabled = true;

        yield return StartCoroutine(FirstPointPatrolling());

        targetPosition = firstPointPatrolling;
        var direction = targetPosition - transform.position;
        kinematicVelocity = direction.normalized * monsterData.patrolSpeed;

        while (CurrentState == MonsterState.Patrol)
        {
            // Ahora se espera: hasta llegar a este punto o haya pasado un tiempo máximo
            float secondsSameDirection = Random.Range(monsterData.minSecondsSameDirection, monsterData.maxSecondsSameDirection);
            float maxTimeInSameDirection = Time.time + secondsSameDirection;
            
            yield return new WaitUntil(() => 
                   Time.time > maxTimeInSameDirection 
                || Vector3.Distance(transform.position, targetPosition) < monsterData.minDistanceToTarget
                || DistanceToPlayer < monsterData.minDistanceToPlayer);

            GetDirectionAndTargetPositionPatrolling(out direction, out targetPosition, monsterData.randomPositionBehindCenter, monsterData.maxHeightPatrolling);
            kinematicVelocity = direction.normalized * monsterData.patrolSpeed;
        }
        
    }
    

    IEnumerator FirstPointPatrolling()
    {
        // El primer punto a elegir debe ser especial, para que el monstruo empiece alejándose del player
        var r = monsterData.spherePatrollingRadius;
        var h = monsterData.spherePatrollingHeight;
        var d = monsterData.spherePatrollingDistanceToTarget;

        for (int i = 0; i < monsterData.firstPointMaxAttempts; i++)
        {
            if (monsterData.patrolMode == PatrolMode.InsideSphere)
                firstPointPatrolling = GetRandomPositionInsideSphere(GameManager.Instance.Portal, r, h, d);
            else
                firstPointPatrolling = GetRandomPositionOnSphere(GameManager.Instance.Portal, r, h, d);

            Vector3 a = GameManager.Instance.PlayerForward;
            Vector3 b = firstPointPatrolling - GameManager.Instance.PlayerPosition;
            a.Normalize();
            b.Normalize();
            float dot = Vector3.Dot(a, b);
            
            if (dot >= monsterData.firstPointMinDot)
                break;

            yield return null;
        }
        
        yield break;
    }
   

    IEnumerator AttackRoutine()
    {
        anim.SetTrigger("Pursue");

        GameManager.Instance.MonsterAttacking(this);

        // Cada x seg se va ajustando la dirección hacia el player
        while (CurrentState == MonsterState.Attack)
        {
            var direction = GameManager.Instance.PlayerPosition - transform.position;
            kinematicVelocity = direction.normalized * monsterData.attackSpeed;
            yield return new WaitForSeconds(monsterData.secondsToAdjustDirection);
        }
    }           

}
