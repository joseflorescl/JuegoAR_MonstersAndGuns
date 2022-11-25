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
        if (getPlayerPos && GameManager.Instance.PlayerPosition() != Vector3.zero)
        {
            print(Time.frameCount + " Player pos en Update == " + GameManager.Instance.PlayerPosition());
            var direction = GameManager.Instance.PlayerPosition() - transform.position;
            direction.Normalize();

            kinematicVelocity = direction * speed;

            getPlayerPos = false;
        }
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
