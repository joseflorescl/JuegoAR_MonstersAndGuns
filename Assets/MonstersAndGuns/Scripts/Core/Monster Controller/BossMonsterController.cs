using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterController : BaseMonsterController
{
    [SerializeField] private float minSecondsPatrolState = 3f;
    [SerializeField] private float maxSecondsPatrolState = 5f;

    [SerializeField] private float minSecondsAttackState = 3f;
    [SerializeField] private float maxSecondsAttackState = 5f;

    [SerializeField] private int firesPerSecond = 1;

    MonsterShooterController shooterController;

    protected override void Awake()
    {
        base.Awake();
        shooterController = GetComponent<MonsterShooterController>();
    }

    private void Start()
    {
        CurrentState = MonsterState.GoUp;
    }    

    protected override void Attack()
    {
        StartCoroutine(BossAttackRoutine());
    }

    protected override void GoUp()
    {
        StartCoroutine(BossGoUpCoroutine());
    }

    protected override void Idle()
    {
        //Nada
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
        var r = monsterData.spherePatrollingRadius;
        var h = monsterData.spherePatrollingHeight;
        var d = monsterData.spherePatrollingDistanceToTarget;

        float secondsInPatrol = Random.Range(minSecondsPatrolState, maxSecondsPatrolState);
        float maxTimeInPatrol = Time.time + secondsInPatrol;

        while (CurrentState == MonsterState.Patrol)
        {
            var targetPosition = GetRandomPositionInsideSphere(GameManager.Instance.Portal, r, h, d, behind: true);
            var direction = targetPosition - transform.position;
            kinematicVelocity = direction.normalized * monsterData.patrolSpeed;

            // Ahora se espera: hasta llegar a este punto o haya pasado un tiempo máximo
            float secondsSameDirection = Random.Range(monsterData.minSecondsSameDirection, monsterData.maxSecondsSameDirection);
            float maxTimeInSameDirection = Time.time + secondsSameDirection;

            yield return new WaitUntil(() => 
                   Time.time > maxTimeInSameDirection 
                || Time.time > maxTimeInPatrol
                || Vector3.Distance(transform.position, targetPosition) < monsterData.minDistanceToTarget);

            if (Time.time > maxTimeInPatrol)
                CurrentState = MonsterState.Attack;
        }

    }


    IEnumerator BossAttackRoutine()
    {
        float secondsInAttack = Random.Range(minSecondsAttackState, maxSecondsAttackState);
        float maxTimeInAttack = Time.time + secondsInAttack;

        StartCoroutine(FireProjectileRoutine());

        while (CurrentState == MonsterState.Attack)
        {
            var direction = GameManager.Instance.PlayerPosition - transform.position;
            kinematicVelocity = direction.normalized * monsterData.attackSpeed;

            float secondsSameDirection = (monsterData.secondsToAdjustDirection);
            float maxTimeInSameDirection = Time.time + secondsSameDirection;

            yield return new WaitUntil(() =>
                 Time.time > maxTimeInSameDirection
              || Time.time > maxTimeInAttack);

            if (Time.time > maxTimeInAttack)
                CurrentState = MonsterState.Patrol;
        }            
    }

    IEnumerator FireProjectileRoutine()
    {
        float fireRate = 1f / firesPerSecond;

        while (CurrentState == MonsterState.Attack)
        {
            yield return new WaitForSeconds(fireRate);
            shooterController.FireToTarget(GameManager.Instance.PlayerPosition);
        }
    }
}
