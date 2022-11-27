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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        rend = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        if (monsterData.speed == 0)
            CurrentState = MonsterState.Idle;
        else
            CurrentState = MonsterState.GoUp;
    }

    IEnumerator GoUpCoroutine()
    {
        coll.enabled = false; // Primero se le desactiva el collider mientras el monstruo va subiendo, y se activará al inicio del Patrol

        kinematicVelocity = GetRandomVectorUp(monsterData.maxDeviationRandomVectorUp) * monsterData.speed;

        if (monsterData.faceInitialDirection)
            transform.rotation = Quaternion.LookRotation(kinematicVelocity);
        
        float secondsGoUp = Random.Range(monsterData.minSecondsGoUp, monsterData.maxSecondsGoUp);
        yield return new WaitForSeconds(secondsGoUp);
        
        CurrentState = MonsterState.Patrol;
    }


    IEnumerator PatrolRoutine()
    {
        coll.enabled = true;
        var portal = GameManager.Instance.Portal();

        /*
        // TODO: el primer punto a elegir debe ser especial, para que el monstruo empiece alejándose del player
        //  Se puede usar esta idea:
        Vector3 a = player.forward;
        Vector3 b = point.position - player.position;
        a.Normalize();
        b.Normalize();
        float dot = Vector3.Dot(a, b);

        //  el primer punto de patrolling puede ser un punto que cumpla que su Dot(a,b) >= 0.4
        // Si despues de x intentos no encontramos nada se elige el ultimo punto encontrado
        */

        var r = monsterData.spherePatrollingRadius;
        var h = monsterData.spherePatrollingHeight - portal.position.y; // Se resta la altura del portal para que sea la altura c/r al suelo, no c/r al portal
        //var h = monsterData.spherePatrollingHeight;
        var d = monsterData.spherePatrollingDistanceToPortal;

        while (currentState == MonsterState.Patrol)
        {
            var targetPosition = GetRandomPositionOnSphere(r, h, d);
           
            // Nota importante sobre TransformPoint: si el objeto portal tiene valores != 1 en la escala, el valor resultante no será el esperado
            targetPosition = portal.transform.TransformPoint(targetPosition);  // Esta posición ahora se debe orientar c/r al portal

            var direction = targetPosition - transform.position;
            kinematicVelocity = direction.normalized * monsterData.speed;

            // Ahora se espera: hasta llegar a este punto o haya pasado un tiempo máximo
            float secondsSameDirection = Random.Range(monsterData.minSecondsSameDirection, monsterData.maxSecondsSameDirection);
            float maxTimeInSameDirection = Time.time + secondsSameDirection;
            
            yield return new WaitUntil(() => Time.time > maxTimeInSameDirection || Vector3.Distance(transform.position, targetPosition) < monsterData.minDistanceToTarget);
        }
        
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
            var direction = GameManager.Instance.PlayerPosition() - transform.position;
            kinematicVelocity = direction.normalized * monsterData.attackSpeed;
            yield return new WaitForSeconds(monsterData.secondsToAdjustDirection);
        }
    }

    private void Update()
    {
        if (CurrentState == MonsterState.Idle) return;

        RotateToVelocity();
    }

    private void FixedUpdate()
    {
        if (CurrentState == MonsterState.Idle) return;

        rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
    }

    void RotateToVelocity()
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
        var offset = new Vector3(0f, height, distance);

        Vector3 targetPosition = Random.onUnitSphere * radius; // Se elige un punto aleatorio en la superficie de la esfera de radio r

        if (!under)
            targetPosition.y = Mathf.Abs(targetPosition.y);
        if (!behind)
            targetPosition.z = Mathf.Abs(targetPosition.z);

        targetPosition += offset;
        return targetPosition;

        //TODO: estoy haciendo pruebas y al parecer la height NO debe ser relativa al portal sino que a la altura de World
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, kinematicVelocity);
    }

}
