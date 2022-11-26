using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "Monsters n Guns/Monster Data")]
public class MonsterData : ScriptableObject
{
    public float speed = 2f;
    public float turnSpeed = 90f;

    [Header("Go Up State")]
    //[Space(10)]
    public bool faceInitialDirection = true;
    public float maxDeviationRandomVectorUp = 0.5f;
    public float minSecondsGoUp = 1f;
    public float maxSecondsGoUp = 3f;

    [Header("Patrol State")]
    //[Space(10)]
    public float spherePatrollingRadius = 2f;
    public float spherePatrollingDistanceCenterToPlayer = 1f; // Al inicio del juego puede ser 1, pero luego para más dificultad puede ser incluso negativa.
    public float spherePatrollingHeight = 0.5f; // Puede mer mejor colocarla casi a la altura del piso.
    // El centro de la esfera de patrullaje estará dado por la fórmula:
    //  Vector3 centerSphere = new Vector3(playerPosition.x, spherePatrollingHeight, playerPosition.z + spherePatrollingDistanceCenterToPlayer);


    //public float maxAngleRotationForward = 45f; // TODO: borrar esta var
    public float minSecondsSameDirection = 2f;
    public float maxSecondsSameDirection = 4f;
    
    //public float minHeightPatroling = 0.5f; // TODO: borrar estas vars
    //public float maxHeightPatroling = 2.5f;
    public float minDistanceToTarget = 0.1f;

    public MonsterColor initialColor = MonsterColor.White;
    public MonsterColor attackColor = MonsterColor.Red;
    public Material attackMaterial;
}
