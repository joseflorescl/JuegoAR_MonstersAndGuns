using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "Monsters n Guns/Monster Data")]
public class MonsterData : ScriptableObject
{
    public enum RotateTowardsMode { Player, Velocity, None}
    
    public float turnSpeed = 90f;
    public int scorePoints = 100;
    public RotateTowardsMode rotateTowardsMode = RotateTowardsMode.Velocity;
    public float speedAnimation = 1;
    public float minDistanceToPlayer = 1f;

    [Header("Go Up State")]
    public float goUpSpeed = 2f;
    public bool faceInitialDirection = true;
    public float maxDeviationRandomVectorUp = 0.5f;
    public float minSecondsGoUp = 1f;
    public float maxSecondsGoUp = 3f;
    public float angleToAwayFromPlayer = 30f;

    [Header("Patrol State")]
    public PatrolMode patrolMode = PatrolMode.OnSphere;
    public bool randomPositionBehindCenter = false;
    public float patrolSpeed = 2f;
    public int firstPointMaxAttempts = 3;
    public float firstPointMinDot = 0.5f;
    public float spherePatrollingRadius = 2f;
    public float spherePatrollingDistanceToTarget = 1f;
    public float spherePatrollingHeight = 0.5f;
    public float maxHeightPatrolling = 3f;
    public float minSecondsSameDirection = 2f;
    public float maxSecondsSameDirection = 4f;
    public float minDistanceToTarget = 0.1f;

    [Header("Attack State")]
    public float attackSpeed = 2.5f;
    public float secondsToAdjustDirection = 1f;
    public Color initialColor = Color.white;
    public Color attackColor = Color.red;
    public Material attackMaterial;
    public Material damageMaterial;
    public Material normalMaterial;
}
