using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : BaseMonsterController
{    
    private void Start()
    {
        GameManager.Instance.MonsterCreated(this);

        if (monsterData.speed == 0)
            CurrentState = MonsterState.Idle;
        else
            CurrentState = MonsterState.GoUp;
    }

    protected override void Idle()
    {
        // NADA
    }

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

        kinematicVelocity = GetRandomVectorUp(monsterData.maxDeviationRandomVectorUp) * monsterData.speed;
        FaceInitialDirection();

        float secondsGoUp = Random.Range(monsterData.minSecondsGoUp, monsterData.maxSecondsGoUp);
        float maxTime = Time.time + secondsGoUp;

        yield return new WaitWhile(() => (Time.time < maxTime) 
                                      && (Vector3.Distance(transform.position, GameManager.Instance.PlayerPosition) > monsterData.minDistanceToPlayer));

        if (Time.time < maxTime) // Entonces el yield anterior terminó porque el monstruo está muy cerca del player
        {
            var direction = GetDirectionAwayFromPlayer();
            kinematicVelocity = direction * monsterData.speed;
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
        var r = monsterData.spherePatrollingRadius;
        var h = monsterData.spherePatrollingHeight;
        var d = monsterData.spherePatrollingDistanceToPortal;

        yield return StartCoroutine(FirstPointPatrolling());

        var targetPosition = firstPointPatrolling;

        while (CurrentState == MonsterState.Patrol)
        {
            var direction = targetPosition - transform.position;
            kinematicVelocity = direction.normalized * monsterData.speed;

            // Ahora se espera: hasta llegar a este punto o haya pasado un tiempo máximo
            float secondsSameDirection = Random.Range(monsterData.minSecondsSameDirection, monsterData.maxSecondsSameDirection);
            float maxTimeInSameDirection = Time.time + secondsSameDirection;
            
            yield return new WaitUntil(() => Time.time > maxTimeInSameDirection || Vector3.Distance(transform.position, targetPosition) < monsterData.minDistanceToTarget);

            targetPosition = GetRandomPositionOnSphere(r, h, d);
        }
        
    }
    

    IEnumerator FirstPointPatrolling()
    {
        // El primer punto a elegir debe ser especial, para que el monstruo empiece alejándose del player
        var r = monsterData.spherePatrollingRadius;
        var h = monsterData.spherePatrollingHeight;
        var d = monsterData.spherePatrollingDistanceToPortal;

        for (int i = 0; i < monsterData.firstPointMaxAttempts; i++)
        {
            firstPointPatrolling = GetRandomPositionOnSphere(r, h, d);
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

        var rend = GetComponentInChildren<Renderer>();
        rend.material = monsterData.attackMaterial;

        // Cada x seg se va ajustando la dirección hacia el player
        while (true)
        {
            var direction = GameManager.Instance.PlayerPosition - transform.position;
            kinematicVelocity = direction.normalized * monsterData.attackSpeed;
            yield return new WaitForSeconds(monsterData.secondsToAdjustDirection);
        }
    }

    Vector3 GetRandomVectorUp(float maxDeviationRandomVectorUp)
    {
        float x = Random.Range(-maxDeviationRandomVectorUp, +maxDeviationRandomVectorUp);
        float z = Random.Range(-maxDeviationRandomVectorUp, +maxDeviationRandomVectorUp);
        float y = 1f;
        var goUpVector = new Vector3(x, y, z);
        goUpVector.Normalize();
        return goUpVector;
    }

    Vector3 GetRandomPositionOnSphere(float radius, float height, float distance, bool under = false, bool behind = false)
    {
        var portal = GameManager.Instance.Portal;
        var offset = new Vector3(0f, height - portal.position.y, distance); // Por ahora se está probando con una altura c/r al mundo, NO c/r al portal

        Vector3 targetPosition = Random.onUnitSphere * radius; // Se elige un punto aleatorio en la superficie de la esfera de radio r

        if (!under)
            targetPosition.y = Mathf.Abs(targetPosition.y);
        if (!behind)
            targetPosition.z = Mathf.Abs(targetPosition.z);

        targetPosition += offset;

        // Nota importante sobre TransformPoint: si el objeto portal tiene valores != 1 en la escala, el valor resultante no será el esperado
        targetPosition = portal.transform.TransformPoint(targetPosition);  // Esta posición ahora se debe orientar c/r al portal

        return targetPosition;
    }

}
