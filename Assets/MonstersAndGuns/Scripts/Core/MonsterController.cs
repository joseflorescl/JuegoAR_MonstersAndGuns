using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MonsterColor { White, Red, Green, Blue, Yellow }
public enum MonsterState { Idle, GoUp, Patrol, Attack }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class MonsterController : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;

    MonsterState currentState;

    public int Score => monsterData.scorePoints;
    public MonsterColor CurrentColor => currentState == MonsterState.Attack ? monsterData.attackColor : monsterData.initialColor;
    public Vector3 ExplosionPosition => rend.bounds.center; // Por ahora es el centro del mesh renderer, pero se podría elegir otra posicion adhoc

    public MonsterState CurrentState
    {
        get { return currentState; }
        private set
        {
            currentState = value;
            StopAllCoroutines();

            switch (currentState)
            {
                case MonsterState.Idle:
                    break; // No se hace nada
                case MonsterState.GoUp:
                    StartCoroutine(GoUpCoroutine());
                    break;
                case MonsterState.Patrol:
                    StartCoroutine(PatrolRoutine());
                    break;
                case MonsterState.Attack:
                    StartCoroutine(AttackRoutine());
                    break;
                default:
                    break;
            }
        }
    }

    Rigidbody rb;
    Animator anim;
    Collider coll;
    Renderer rend;
    Vector3 kinematicVelocity;
    Vector3 firstPointPatrolling;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        rend = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        GameManager.Instance.MonsterCreated(this);

        if (monsterData.speed == 0)
            CurrentState = MonsterState.Idle;
        else
            CurrentState = MonsterState.GoUp;
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

    void FaceInitialDirection()
    {
        if (monsterData.faceInitialDirection)
            transform.rotation = Quaternion.LookRotation(kinematicVelocity);
    }

    IEnumerator PatrolRoutine()
    {
        coll.enabled = true;
        var r = monsterData.spherePatrollingRadius;
        var h = monsterData.spherePatrollingHeight;
        var d = monsterData.spherePatrollingDistanceToPortal;

        yield return StartCoroutine(FirstPointPatrolling());

        var targetPosition = firstPointPatrolling;

        while (currentState == MonsterState.Patrol)
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

    public void Attack()
    {
        CurrentState = MonsterState.Attack; // El paso a estado Attack es controlado por el BattleManager
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

    private void Update()
    {
        if (CurrentState == MonsterState.Idle) return;

        RotateTowardsToVelocity();
    }

    private void FixedUpdate()
    {
        if (CurrentState == MonsterState.Idle) return;

        rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
    }

    void RotateTowardsToVelocity()
    {
        var deltaRotation = monsterData.turnSpeed * Time.deltaTime;
        var targetRotation = Quaternion.LookRotation(kinematicVelocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, deltaRotation);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, kinematicVelocity);
    }

}
