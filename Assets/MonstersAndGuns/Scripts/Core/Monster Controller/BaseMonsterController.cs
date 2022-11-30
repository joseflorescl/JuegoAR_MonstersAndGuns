using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterColor { White, Red, Green, Blue, Yellow }
public enum MonsterState { Idle, GoUp, Patrol, Attack }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public abstract class BaseMonsterController : MonoBehaviour
{
    [SerializeField] protected MonsterData monsterData;

    public int Score => monsterData.scorePoints;
    public MonsterColor CurrentColor => CurrentState == MonsterState.Attack ? monsterData.attackColor : monsterData.initialColor;
    public Vector3 ExplosionPosition => rend.bounds.center; // Por ahora es el centro del mesh renderer, pero se podría elegir otra posicion adhoc

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
    protected Vector3 kinematicVelocity;
    protected Vector3 firstPointPatrolling;

    protected abstract void Idle();
    protected abstract void GoUp();
    protected abstract void Patrol();
    protected abstract void Attack();

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        rend = GetComponentInChildren<Renderer>();
    }

    protected void FaceInitialDirection()
    {
        if (monsterData.faceInitialDirection)
            transform.rotation = Quaternion.LookRotation(kinematicVelocity);
    }

    protected void RotateTowardsVelocity()
    {
        var deltaRotation = monsterData.turnSpeed * Time.deltaTime;
        var targetRotation = Quaternion.LookRotation(kinematicVelocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, deltaRotation);
    }

    protected void Update()
    {
        if (CurrentState == MonsterState.Idle) return;

        if (monsterData.rotateTowardsVelocity)
            RotateTowardsVelocity();
    }

    protected void FixedUpdate()
    {
        if (CurrentState == MonsterState.Idle) return;

        rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, kinematicVelocity);
    }

}
