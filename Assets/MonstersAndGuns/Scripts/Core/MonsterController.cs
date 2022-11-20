using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterController : MonoBehaviour
{

    [SerializeField] private float maxDeviationRandomVectorUp = 0.5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool faceInitialDirection = true;

    [SerializeField] private float minSecondsGoUp = 1f;
    [SerializeField] private float maxSecondsGoUp = 3f;
    [SerializeField] private float maxAngleRotationPlayerForward = 45f;



    enum MonsterStates { GoUp, Patrol, Attack}

    // TODO: hace la var state una property de tal forma que al setear su valor se llame al StartCoroutine, que da m�s elegante 
    //  que se haga directamente un StartCoroutine dentro de otra corutina.
    MonsterStates currentState;
    Rigidbody rb;
    Vector3 kinematicVelocity;

    MonsterStates CurrentState
    {
        get { return currentState; }
        set
        {
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
                    print("Attack Pendiente");
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
    }


    IEnumerator GoUpCoroutine()
    {
        // Se elige un vector hacia arriba random
        float x = Random.Range(-maxDeviationRandomVectorUp, +maxDeviationRandomVectorUp);
        float z = Random.Range(-maxDeviationRandomVectorUp, +maxDeviationRandomVectorUp);
        float y = 1f;
        goUpVector = new Vector3(x, y, z);
        goUpVector.Normalize();

        // Ahora se mueve al enemigo en esa direcci�n, usando un rb kinem�tico:
        // Notar que un gameObject kinem�tico 3D NO se movera si se setea su rb.velocity, a diff de un Rb2D que s� me mover� al objeto.
        // Hay que usar MovePosition en el FixedUpdate

        // Notar que si NO se usa interpolaci�n, siempre tendremos que transform.position == rb.position
        //  pero al usar interpolaci�n, en el Update veremos que a veces transform.position != rb.position, pero en el
        //  FixedUpdate ambos valores siempre ser�n iguales

        // Esto es interesante: cuando el objeto es kinem�tico, pero la interpolaci�n se setea a None,
        //  al moverlo en el FixedUpdate el objeto solo se mover� hasta alcanzar la posici�n del vector de velocidad, no se seguir� moviendo.
        //  En conclusi�n: si el objeto es un rb3D kinem�tico, se debe setear la interpolaci�n.
        kinematicVelocity = goUpVector * speed;

        if (faceInitialDirection)
            transform.rotation = Quaternion.LookRotation(goUpVector);
        
        float secondsGoUp = Random.Range(minSecondsGoUp, maxSecondsGoUp);

        yield return new WaitForSeconds(secondsGoUp);

        CurrentState = MonsterStates.Patrol;
    }


    IEnumerator PatrolRoutine()
    {
        // Primero nos alejamos del player usando la direcci�n player.forward, pero rotada en un �ngulo random
        float angle = Random.Range(-maxAngleRotationPlayerForward, +maxAngleRotationPlayerForward);
        var deltaRotation = Quaternion.Euler(0, angle, 0);
        kinematicVelocity = deltaRotation * GameManager.Instance.PlayerForwardDirection() * speed;
        yield return null; 

        // Ahora se espera unos segundos hasta que se comience en un loop de patrol alrededor del player
        // TODO: ir rotando con el Slerp/RotateTowards el enemigo para que su vector forward se vaya alineando con su velocidad
    }
    

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Vector3.zero, goUpVector);
    }

}
