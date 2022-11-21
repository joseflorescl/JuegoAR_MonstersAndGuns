using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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

    public float TimeToAttack { get; set; }


    enum MonsterStates { GoUp, Patrol, Attack}

    // TODO: hace la var state una property de tal forma que al setear su valor se llame al StartCoroutine, que da más elegante 
    //  que se haga directamente un StartCoroutine dentro de otra corutina.
    MonsterStates currentState;
    Rigidbody rb;
    Vector3 kinematicVelocity;
    

    MonsterStates CurrentState
    {
        // TODO: implementar una property de este estilo para la clase GameManager
        get { return currentState; }
        set
        {
            StopAllCoroutines();
            currentState = value;

            switch (currentState)
            {
                case MonsterStates.GoUp:
                    StartCoroutine(GoUpCoroutine());
                    break;
                case MonsterStates.Patrol:
                    StartCoroutine(PatrolRoutine());
                    break;
                case MonsterStates.Attack:
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
    }

    private void Start()
    {
        CurrentState = MonsterStates.GoUp;

        // TODO: test: esto lo debe setear el spawner al crear el monster:
        TimeToAttack = Time.time + Random.Range(5f, 20f);
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

        CurrentState = MonsterStates.Patrol;
    }


    IEnumerator PatrolRoutine()
    {
        // Primero nos alejamos del player usando la dirección portal.forward, pero rotada en un ángulo random
        float angle = Random.Range(-maxAngleRotationForward, +maxAngleRotationForward);
        var deltaRotation = Quaternion.Euler(0, angle, 0);
        kinematicVelocity = deltaRotation * GameManager.Instance.PortalForwardDirection() * speed;
        
        yield return null;


        while (true)
        {
            // Ahora se espera unos segundos
            float secondsSameDirection = Random.Range(minSecondsSameDirection, maxSecondsSameDirection);

            float maxTimeInSameDirection = Time.time + secondsSameDirection;
            
            yield return new WaitUntil(() => Time.time > maxTimeInSameDirection || transform.position.y < minHeightPatroling);

            // Se elige un punto alrededor del player: en una esfera de radio r
            // Se elige un punto random dentro de la semiesfera con origen en el player.position de rado r
            var pos = Random.insideUnitSphere * radiusSpherePatroling + GameManager.Instance.PlayerPosition();
            pos.y = Mathf.Abs(pos.y);
            var direction = pos - transform.position;
            kinematicVelocity = direction.normalized * speed;

            // TODO: pasar a estado Attack después de un rato: tal vez lo mejor es que ese valor lo setee el que crea el monster
            //  porque en el juego original el paso a ataque no es tan aleatorio, sino que van de a uno pasando al ataque.
            // Al ser inicializado el monster, el spawner le va a setear la cantidad de segundos que va a estar antes de pasar al ataque.
            // TODO: este if debería ir en el while
            if (Time.time > TimeToAttack)
            {
                CurrentState = MonsterStates.Attack;
            }
        }
        
    }

    IEnumerator AttackRoutine()
    {
        var direction = GameManager.Instance.PlayerPosition() - transform.position;
        kinematicVelocity = direction.normalized * speed;

        yield return new WaitForSeconds(5);
        Destroy(gameObject);
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

        //var color = Color.red;
        //color.a = 0.02f;
        //Gizmos.color = color;
        //Gizmos.DrawSphere(Vector3.zero, radiusSpherePatroling);
    }

}
