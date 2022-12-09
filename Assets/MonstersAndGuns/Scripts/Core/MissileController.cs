using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MissileController : MonoBehaviour, IVFXEntity
{
    [SerializeField] private float speed = 1f;
    [field: SerializeField] public Color CurrentColor { get; private set; }
    public Vector3 ExplosionPosition => transform.position;
    public Material DamageMaterial => null;
    public Material AttackMaterial => null;
    public Material NormalMaterial => null;
    public Renderer[] Renderers => null;

    Rigidbody rb;
    Vector3 kinematicVelocity;
    HealthController healthController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        healthController = GetComponent<HealthController>();
    }
    

    private void OnEnable()
    {
        GameManager.Instance.OnBossMonsterDead += BossMonsterDeadHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnBossMonsterDead -= BossMonsterDeadHandler;
    }

    private void Start() => kinematicVelocity = transform.forward * speed;
    
    private void FixedUpdate() => rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);

    private void BossMonsterDeadHandler(BaseMonsterController obj)
    {
        healthController.Damage(healthController.Health, DamageMode.Collision);
    }
}
