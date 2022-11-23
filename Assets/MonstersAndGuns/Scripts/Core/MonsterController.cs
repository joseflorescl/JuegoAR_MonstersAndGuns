using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class MonsterController : MonoBehaviour
{

    [SerializeField] private float maxDeviationRandomVectorUp = 0.5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private bool faceInitialDirection = true;

    [SerializeField] private float minSecondsGoUp = 1f;
    [SerializeField] private float maxSecondsGoUp = 3f;
    [SerializeField] private float maxAngleRotationForward = 45f;

    [SerializeField] private float minSecondsSameDirection = 2f;
    [SerializeField] private float maxSecondsSameDirection = 4f;
    [SerializeField] private float radiusSpherePatroling = 20f;

    [SerializeField] private float minHeightPatroling = 0.5f;
    [SerializeField] private float minDistanceToTarget = 0.1f;


    public enum MonsterState { GoUp, Patrol, Attack }

    // TODO: hace la var state una property de tal forma que al setear su valor se llame al StartCoroutine, que da más elegante 
    //  que se haga directamente un StartCoroutine dentro de otra corutina.
    MonsterState currentState;
    Rigidbody rb;
    Animator anim;
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

    public Vector3 goUpVector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        CurrentState = MonsterState.GoUp;
    }


    IEnumerator GoUpCoroutine()
    {
        // Se elige un vector hacia arriba random
        float x = Random.Range(-maxDeviationRandomVectorUp, +maxDeviationRandomVectorUp);
        float z = Random.Range(-maxDeviationRandomVectorUp, +maxDeviationRandomVectorUp);
        float y = 1f;
        goUpVector = new Vector3(x, y, z);
        goUpVector.Normalize();

        // Ahora se mueve al enemigo en esa dirección, usando un rb kinemático:
        // Notar que un gameObject kinemático 3D NO se movera si se setea su rb.velocity, a diff de un Rb2D que sí me moverá al objeto.
        // Hay que usar MovePosition en el FixedUpdate

        // Notar que si NO se usa interpolación, siempre tendremos que transform.position == rb.position
        //  pero al usar interpolación, en el Update veremos que a veces transform.position != rb.position, pero en el
        //  FixedUpdate ambos valores siempre serán iguales

        // Esto es interesante: cuando el objeto es kinemático, pero la interpolación se setea a None,
        //  al moverlo en el FixedUpdate el objeto solo se moverá hasta alcanzar la posición del vector de velocidad, no se seguirá moviendo.
        //  En conclusión: si el objeto es un rb3D kinemático, se debe setear la interpolación.
        kinematicVelocity = goUpVector * speed;
        

        if (faceInitialDirection)
            transform.rotation = Quaternion.LookRotation(kinematicVelocity);
        
        float secondsGoUp = Random.Range(minSecondsGoUp, maxSecondsGoUp);

        yield return new WaitForSeconds(secondsGoUp);

        CurrentState = MonsterState.Patrol;
    }


    IEnumerator PatrolRoutine()
    {
        // Primero nos alejamos del player usando la dirección portal.forward, pero rotada en un ángulo random
        float angle = Random.Range(-maxAngleRotationForward, +maxAngleRotationForward);
        var deltaRotation = Quaternion.Euler(0, angle, 0);
        kinematicVelocity = deltaRotation * GameManager.Instance.PortalForwardDirection() * speed;

        float secondsSameDirection = Random.Range(minSecondsSameDirection, maxSecondsSameDirection);
        yield return new WaitForSeconds(secondsSameDirection);

        while (currentState == MonsterState.Patrol)
        {
            // Se elige un punto al frente del player (casi, puede ser un poco por detrás también): en una esfera de radio r
            // Se elige un punto random dentro de la semiesfera con origen en el player.position de rado r
            
            var targetPosition = Random.insideUnitSphere * radiusSpherePatroling + GameManager.Instance.PlayerPosition();
            targetPosition.y = Mathf.Abs(targetPosition.y);
            targetPosition.z = Mathf.Abs(targetPosition.z);
            // TODO: falta hacer que la targetPosition pueda estar también un poco por detrás del player: ahora siempre estará por delante
            // TODO: también se puede setear una altura máxima, porque en el eje X puede estar bien la distancia, pero hacia arriba es mejor algo menos.
            var direction = targetPosition - transform.position;
            kinematicVelocity = direction.normalized * speed;

            // Ahora se espera: hasta llegar a este punto o haya pasado un tiempo máximo
            secondsSameDirection = Random.Range(minSecondsSameDirection, maxSecondsSameDirection);
            float maxTimeInSameDirection = Time.time + secondsSameDirection;
            yield return new WaitUntil(() => Time.time > maxTimeInSameDirection || Vector3.Distance(transform.position, targetPosition) < minDistanceToTarget);
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
        rend.material.color = Color.red;

        Invoke(nameof(Dead), 3);

        // Cada 1 seg se va ajustando la dirección hacia el player
        while (true)
        {
            var direction = GameManager.Instance.PlayerPosition() - transform.position;
            kinematicVelocity = direction.normalized * speed;
            yield return new WaitForSeconds(1);
        }
    }


    private void Dead()
    {
        StopAllCoroutines();
        Destroy(gameObject); // TODO: test!
        GameManager.Instance.EnemyDead(this);
    }

    private void Update()
    {
        // Rotación en dirección de su velocidad
        var step = turnSpeed * Time.deltaTime;
        var targetRotation = Quaternion.LookRotation(kinematicVelocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);

        
    }


    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, kinematicVelocity);
    }

}
