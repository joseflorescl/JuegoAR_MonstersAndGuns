using UnityEngine;

public enum MonsterState { Idle, GoUp, Patrol, Attack }
public enum PatrolMode { OnSphere, InsideSphere }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(HealthController))]
public abstract class BaseMonsterController : MonoBehaviour, IVFXEntity
{
    [SerializeField] protected MonsterData monsterData;

    public int Score => monsterData.scorePoints;
    public Color CurrentColor => CurrentState == MonsterState.Attack ? monsterData.attackColor : monsterData.initialColor;
    public Vector3 ExplosionPosition => renderers[0].bounds.center; // Por ahora es el centro del mesh renderer, pero se podría elegir otra posicion adhoc
    public Material DamageMaterial => monsterData.damageMaterial;
    public Material AttackMaterial => monsterData.attackMaterial;
    public Material NormalMaterial => monsterData.normalMaterial;
    public Renderer[] Renderers => renderers;

    public float CurrentHealthPercentage => healthController.CurrentHealthPercentage;

    private MonsterState currentState;

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

    protected float DistanceToPlayer => Vector3.Distance(transform.position, GameManager.Instance.PlayerPosition);
    protected float DistanceToPlayerOnPlaneXZ()
    {
        var position = transform.position;
        position.y = 0;
        var playerPosition = GameManager.Instance.PlayerPosition;
        playerPosition.y = 0;
        return Vector3.Distance(position, playerPosition);
    }

    protected Rigidbody rb;
    protected Animator anim;
    protected Collider coll;
    protected Renderer[] renderers;
    protected HealthController healthController;
    protected Vector3 kinematicVelocity;
    protected Vector3 firstPointPatrolling;
    protected Vector3 targetPosition;

    protected abstract void Idle();
    protected abstract void GoUp();
    protected abstract void Patrol();
    protected abstract void Attack();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
        healthController = GetComponent<HealthController>();
    }

    //protected virtual void Start() // TODO: eliminar esta línea
    protected virtual void Init()
    {
        anim.speed = monsterData.speedAnimation;        
    }

    protected virtual void OnEnable()
    {
        Init(); // Pool Manager Compatible
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerDead -= PlayerDeadHandler;
    }

    protected virtual void PlayerDeadHandler()
    {
        CurrentState = MonsterState.Patrol;
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

    protected virtual void Update()
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

    protected virtual void FixedUpdate()
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
        Vector3 targetPosition = randomValue * radius;

        // Por ahora se está probando con una altura c/r al mundo, NO c/r al target: por eso se resta
        var offset = new Vector3(0f, height - target.position.y, distance); 

        if (!under)
            targetPosition.y = Mathf.Abs(targetPosition.y);
        if (!behind)
            targetPosition.z = Mathf.Abs(targetPosition.z);

        targetPosition += offset;

        // Nota importante sobre TransformPoint: si el objeto target tiene valores != 1 en la escala, el valor resultante no será el esperado
        targetPosition = target.transform.TransformPoint(targetPosition);
        return targetPosition;
    }

    protected void GetDirectionAndTargetPositionPatrolling(out Vector3 direction, out Vector3 targetPosition, bool behind, 
        float maxHeight,
        bool useDistanceOnPlaneXZ = false)
    {
        var r = monsterData.spherePatrollingRadius;
        var h = monsterData.spherePatrollingHeight;
        var d = monsterData.spherePatrollingDistanceToTarget;

        var distance = useDistanceOnPlaneXZ ? DistanceToPlayerOnPlaneXZ() : DistanceToPlayer;

        if (distance < monsterData.minDistanceToPlayer)
        {
            direction = transform.position - GameManager.Instance.PlayerPosition;
            targetPosition = transform.position + direction;
        }
        else
        {
            if (monsterData.patrolMode == PatrolMode.InsideSphere)
                targetPosition = GetRandomPositionInsideSphere(GameManager.Instance.Portal, r, h, d, behind: behind);
            else
                targetPosition = GetRandomPositionOnSphere(GameManager.Instance.Portal, r, h, d, behind: behind);

            direction = targetPosition - transform.position;
        }

        targetPosition.y = Mathf.Clamp(targetPosition.y, targetPosition.y, maxHeight);
    }
    
    protected void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, kinematicVelocity);

        if (CurrentState == MonsterState.Patrol)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.2f);
        }

    }

}
