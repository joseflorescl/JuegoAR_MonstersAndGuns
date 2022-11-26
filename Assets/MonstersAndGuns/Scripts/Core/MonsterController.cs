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
        var player = GameManager.Instance.Player();

        // Se calcula el centro de la esfera de patrullaje: siempre estará AL FRENTE DEL PLAYER
        Vector3 centerSphere = player.TransformPoint(Vector3.forward * monsterData.spherePatrollingDistanceCenterToPlayer);
        centerSphere.y = monsterData.spherePatrollingHeight;

        while (currentState == MonsterState.Patrol)
        {
            // Se elige un punto aleatorio en la superficie de la esfera
            var targetPosition = Random.onUnitSphere * monsterData.spherePatrollingRadius;
            // Debo elegir la semiesfera de arriba/adelante: Nota: esto podría ser configurable para darle más dificultad al monstruo
            targetPosition.y = Mathf.Abs(targetPosition.y);
            targetPosition.z = Mathf.Abs(targetPosition.z);
            // Esta posición ahora se debe orientar c/r al player
            targetPosition = player.transform.TransformPoint(targetPosition);
            // Y finalmente esa posición se debe desfasar en ejes Z, Y (en X no lo desfaso porque siempre queremos que esté en el medio del player)
            // Lo desfaso en Z
            targetPosition += player.forward * monsterData.spherePatrollingDistanceCenterToPlayer;
            // Lo desfaso en Y
            targetPosition += player.up * (monsterData.spherePatrollingHeight - player.transform.position.y);


            DebugCreateCube(targetPosition, Vector3.one * 0.1f);

            yield return new WaitForSeconds(0.1f);


            // TODO: falta hacer que la targetPosition pueda estar también un poco por detrás del player: ahora siempre estará por delante
            // TODO: también se puede setear una altura máxima, porque en el eje X puede estar bien la distancia, pero hacia arriba es mejor algo menos.
            //var direction = targetPosition - transform.position;
            //kinematicVelocity = direction.normalized * monsterData.speed;

            // Ahora se espera: hasta llegar a este punto o haya pasado un tiempo máximo
            //secondsSameDirection = Random.Range(monsterData.minSecondsSameDirection, monsterData.maxSecondsSameDirection);
            //float maxTimeInSameDirection = Time.time + secondsSameDirection;
            //yield return new WaitUntil(() => Time.time > maxTimeInSameDirection || Vector3.Distance(transform.position, targetPosition) < monsterData.minDistanceToTarget);
            // El paso a estado Attack es controlado por el BattleManager
        }
        
    }

    void DebugCreateCube(Vector3 position, Vector3 scale)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = scale;
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
