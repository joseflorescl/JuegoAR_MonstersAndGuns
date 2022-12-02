using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState { Idle, GoUp, Patrol, Attack }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(HealthController))]
public abstract class BaseMonsterController : MonoBehaviour, IVFXEntity
{
    [SerializeField] protected MonsterData monsterData;

    public int Score => monsterData.scorePoints;
    public VFXColor CurrentColor => CurrentState == MonsterState.Attack ? monsterData.attackColor : monsterData.initialColor;
    public Vector3 ExplosionPosition => rend.bounds.center; // Por ahora es el centro del mesh renderer, pero se podría elegir otra posicion adhoc
    public float CurrentHealthPercentage => healthController.CurrentHealthPercentage;

    protected MonsterState currentState;

    public MonsterState CurrentState
    {
        get { return currentState; }
        protected set
        {
            currentState = value;
            StopAllCoroutines();

            switch (currentState)
            {
                case MonsterState.Idle:
                    Idle();
                    break; 
                case MonsterState.GoUp:
                    GoUp();
                    break;
                case MonsterState.Patrol:
                    Patrol();
                    break;
                case MonsterState.Attack:
                    Attack();
                    break;                
            }
        }
    }

    protected Rigidbody rb;
    protected Animator anim;
    protected Collider coll;
    protected Renderer rend;
    protected HealthController healthController;
    protected Vector3 kinematicVelocity;
    protected Vector3 firstPointPatrolling;

    protected abstract void Idle();
    protected abstract void GoUp();
    protected abstract void Patrol();
    protected abstract void Attack();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        rend = GetComponentInChildren<Renderer>();
        healthController = GetComponent<HealthController>();
    }

    protected void FaceInitialDirection()
    {
        if (monsterData.faceInitialDirection)
            transform.rotation = Quaternion.LookRotation(kinematicVelocity);
    }

    protected void RotateTowardsVelocity()
    {       
        RotateTowardsLookDirection(kinematicVelocity);
    }

    protected void RotateTowardsPlayer()
    {
        var direction = GameManager.Instance.PlayerPosition - transform.position;
        direction.y = 0f;
        direction.Normalize();
        RotateTowardsLookDirection(direction);
    }

    void RotateTowardsLookDirection(Vector3 lookDirection)
    {
        var deltaRotation = monsterData.turnSpeed * Time.deltaTime;
        var targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, deltaRotation);
    }

    protected void Update()
    {
        if (CurrentState == MonsterState.Idle) return;

        switch (monsterData.rotateTowardsMode)
        {
            case MonsterData.RotateTowardsMode.Player:
                RotateTowardsPlayer();
                break;
            case MonsterData.RotateTowardsMode.Velocity:
                RotateTowardsVelocity();
                break;
            case MonsterData.RotateTowardsMode.None:
                break;
            default:
                break;
        }
    }

    protected void FixedUpdate()
    {
        if (CurrentState == MonsterState.Idle) return;

        rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
    }

    protected Vector3 GetRandomVectorUp(float maxDeviationRandomVectorUp)
    {
        float x = Random.Range(-maxDeviationRandomVectorUp, +maxDeviationRandomVectorUp);
        float z = Random.Range(-maxDeviationRandomVectorUp, +maxDeviationRandomVectorUp);
        float y = 1f;
        var goUpVector = new Vector3(x, y, z);
        goUpVector.Normalize();
        return goUpVector;
    }

    protected Vector3 GetRandomPositionOnSphere(Transform target, float radius, float height, float distance, bool under = false, bool behind = false)
    {
        Vector3 randomValue = Random.onUnitSphere;
        return GetRandomPosition(randomValue, target, radius, height, distance, under, behind);
    }

    protected Vector3 GetRandomPositionInsideSphere(Transform target, float radius, float height, float distance, bool under = false, bool behind = false)
    {
        Vector3 randomValue = Random.insideUnitSphere;
        return GetRandomPosition(randomValue, target, radius, height, distance, under, behind);
    }

    Vector3 GetRandomPosition(Vector3 randomValue, Transform target, float radius, float height, float distance, bool under = false, bool behind = false)
    {
        Vector3 targetPosition = randomValue * radius; // Se elige un punto aleatorio en la superficie de la esfera de radio r

        var offset = new Vector3(0f, height - target.position.y, distance); // Por ahora se está probando con una altura c/r al mundo, NO c/r al target

        if (!under)
            targetPosition.y = Mathf.Abs(targetPosition.y);
        if (!behind)
            targetPosition.z = Mathf.Abs(targetPosition.z);

        targetPosition += offset;

        // Nota importante sobre TransformPoint: si el objeto target tiene valores != 1 en la escala, el valor resultante no será el esperado
        targetPosition = target.transform.TransformPoint(targetPosition);  // Esta posición ahora se debe orientar c/r al target (portal)
        return targetPosition;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, kinematicVelocity);
    }

}
