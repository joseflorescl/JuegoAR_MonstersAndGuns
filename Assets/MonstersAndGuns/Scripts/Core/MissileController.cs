using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MissileController : MonoBehaviour, IVFXEntity
{
    [SerializeField] private float speed = 1f;
    [field: SerializeField] public Color CurrentColor { get; private set; }
    public Vector3 ExplosionPosition => transform.position;

    protected Rigidbody rb;

    Vector3 kinematicVelocity;

    private void Awake() => rb = GetComponent<Rigidbody>();
    
    private void Start() => kinematicVelocity = transform.forward * speed;
    
    private void FixedUpdate() => rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);
}
