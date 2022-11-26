using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxMoveToTarget : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    Rigidbody rb;
    Vector3 kinematicVelocity;
    bool getPlayerPos = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var direction = GameManager.Instance.PlayerPosition() - transform.position;
        direction.Normalize();

        kinematicVelocity = direction * speed;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
        DebugSetMonsterDistance();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, kinematicVelocity);
    }

    void DebugSetMonsterDistance()
    {
        float distance = Vector3.Distance(GameManager.Instance.PlayerPosition(), transform.position);
        FindObjectOfType<DebugManager>().SetMonsterDistance(distance - (1f/2f + 0.25f/2f)); // se le resta el tamaño del player y del monster
    }

}
