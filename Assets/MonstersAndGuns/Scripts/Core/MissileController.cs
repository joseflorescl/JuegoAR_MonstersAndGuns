using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MissileController : MonoBehaviour, IVFXEntity
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float maxDistanceToPlayer = 10f;
    [SerializeField] private float delayValidateDistanceToPlayer = 0.25f;


    [field: SerializeField] public Color CurrentColor { get; private set; }
    public Vector3 ExplosionPosition => transform.position;
    public Material DamageMaterial => null;
    public Material AttackMaterial => null;
    public Material NormalMaterial => null;
    public Renderer[] Renderers => null;

    Rigidbody rb;
    Vector3 kinematicVelocity;
    HealthController healthController;
    WaitForSeconds waitValidateDistanceToPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        healthController = GetComponent<HealthController>();
        waitValidateDistanceToPlayer = new WaitForSeconds(delayValidateDistanceToPlayer);
    }
    

    private void OnEnable()
    {
        GameManager.Instance.OnBossMonsterDead += BossMonsterDeadHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnBossMonsterDead -= BossMonsterDeadHandler;
    }

    private void Start()
    {
        GameManager.Instance.MissileCreated(this);
        kinematicVelocity = transform.forward * speed;

        StartCoroutine(DestroyFarAwayFromPlayerRoutine());
    }

    private void FixedUpdate() => rb.MovePosition(transform.position + kinematicVelocity * Time.deltaTime);

    private void BossMonsterDeadHandler(BaseMonsterController obj)
    {
        healthController.Damage(healthController.Health, DamageMode.Collision);
    }

    IEnumerator DestroyFarAwayFromPlayerRoutine()
    {
        while (true)
        {
            yield return waitValidateDistanceToPlayer;
            if (Vector3.Distance(transform.position, GameManager.Instance.PlayerPosition) > maxDistanceToPlayer)
            {
                healthController.Damage(healthController.Health, DamageMode.Collision);
            }
        }
    }
}
