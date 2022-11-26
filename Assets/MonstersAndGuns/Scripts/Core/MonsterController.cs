using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MonsterColor { White, Red, Green, Blue, Yellow }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class MonsterController : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;


    public enum MonsterState { Idle, GoUp, Patrol, Attack }

    // TODO: hace la var state una property de tal forma que al setear su valor se llame al StartCoroutine, que da más elegante 
    //  que se haga directamente un StartCoroutine dentro de otra corutina.
    MonsterState currentState;
    Rigidbody rb;
    Animator anim;
    Collider coll;
    Vector3 kinematicVelocity;

    public MonsterState CurrentState
    {
        // TODO: implementar una property de este estilo para la clase GameManager
        get { return currentState; }
        private set
        {
            currentState = value;
            StopAllCoroutines();


            switch (currentState)
            {
                case MonsterState.Idle:
                    // No se hace nada
                    break;
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

    public MonsterColor CurrentColor => currentState == MonsterState.Attack ? monsterData.attackColor : monsterData.initialColor;

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
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
        // Primero se le desactiva el collider mientras el monstruo va subiendo, y se activará antes de pasar a Patrol
        coll.enabled = false;

        // Se elige un vector hacia arriba random
        float x = Random.Range(-monsterData.maxDeviationRandomVectorUp, +monsterData.maxDeviationRandomVectorUp);
        float z = Random.Range(-monsterData.maxDeviationRandomVectorUp, +monsterData.maxDeviationRandomVectorUp);
        float y = 1f;
        var goUpVector = new Vector3(x, y, z);
        goUpVector.Normalize();

        // Ahora se mueve al monster en esa dirección, usando un rb kinemático:
        // Notar que un gameObject kinemático 3D NO se movera si se setea su rb.velocity, a diff de un Rb2D que sí me moverá al objeto.
        // Hay que usar MovePosition en el FixedUpdate

        // Notar que si NO se usa interpolación, siempre tendremos que transform.position == rb.position
        //  pero al usar interpolación, en el Update veremos que a veces transform.position != rb.position, pero en el
        //  FixedUpdate ambos valores siempre serán iguales

        // Esto es interesante: cuando el objeto es kinemático, pero la interpolación se setea a None,
        //  al moverlo en el FixedUpdate el objeto solo se moverá hasta alcanzar la posición del vector de velocidad, no se seguirá moviendo.
        //  En conclusión: si el objeto es un rb3D kinemático, se debe setear la interpolación.
        kinematicVelocity = goUpVector * monsterData.speed;
        

        if (monsterData.faceInitialDirection)
            transform.rotation = Quaternion.LookRotation(kinematicVelocity);
        
        float secondsGoUp = Random.Range(monsterData.minSecondsGoUp, monsterData.maxSecondsGoUp);

        yield return new WaitForSeconds(secondsGoUp);
        coll.enabled = true;

        CurrentState = MonsterState.Patrol;
    }


    IEnumerator PatrolRoutine()
    {
        var portal = GameManager.Instance.Portal();

        var offset = new Vector3(0f, monsterData.spherePatrollingHeightToPortal, monsterData.spherePatrollingDistanceToPortal);

        // TODO: el primer punto a elegir puede ser un punto aleatorio sobre una esfera de las mismas dimensiones, pero a una distancia
        // que nos asegure que se alejan del player

        while (currentState == MonsterState.Patrol)
        {
            // Se elige un punto aleatorio en la superficie de la esfera de radio r:
            var targetPosition = Random.onUnitSphere * monsterData.spherePatrollingRadius;
            // Debo elegir la semiesfera de arriba/adelante
            targetPosition.y = Mathf.Abs(targetPosition.y);
            targetPosition.z = Mathf.Abs(targetPosition.z);
            // Y se aplica el offset, por ahora con respecto al origen 0,0,0
            targetPosition += offset;
            // Esta posición ahora se debe orientar c/r al player
            // Nota importante sobre TransformPoint: si el objeto portal tiene valores != 1 en la escala, el valor resultante no será el esperado
            targetPosition = portal.transform.TransformPoint(targetPosition);
            
            var direction = targetPosition - transform.position;
            kinematicVelocity = direction.normalized * monsterData.speed;

            // Ahora se espera: hasta llegar a este punto o haya pasado un tiempo máximo
            float secondsSameDirection = Random.Range(monsterData.minSecondsSameDirection, monsterData.maxSecondsSameDirection);
            float maxTimeInSameDirection = Time.time + secondsSameDirection;

            print("Monster Position: " + transform.position);
            print("Target Position: " + targetPosition);
            print("Distance == " + Vector3.Distance(transform.position, targetPosition));
            yield return new WaitUntil(() => Time.time > maxTimeInSameDirection || Vector3.Distance(transform.position, targetPosition) < monsterData.minDistanceToTarget);
            // El paso a estado Attack es controlado por el BattleManager
        }
        
    }

    public void Attack()
    {
        CurrentState = MonsterState.Attack;
    }

    IEnumerator AttackRoutine()
    {
        anim.SetTrigger("Pursue");

        var rend = GetComponentInChildren<Renderer>();
        rend.material = monsterData.attackMaterial;

        // Cada 1 seg se va ajustando la dirección hacia el player
        while (true)
        {
            var direction = GameManager.Instance.PlayerPosition() - transform.position;
            kinematicVelocity = direction.normalized * monsterData.speed;
            yield return new WaitForSeconds(1);
        }
    }

    private void Update()
    {
        if (CurrentState == MonsterState.Idle) return;

        // Rotación en dirección de su velocidad
        var step = monsterData.turnSpeed * Time.deltaTime;
        var targetRotation = Quaternion.LookRotation(kinematicVelocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }


    private void FixedUpdate()
    {
        if (CurrentState == MonsterState.Idle) return;

        rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, kinematicVelocity);

    }

}
