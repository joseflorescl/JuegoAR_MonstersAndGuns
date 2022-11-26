using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private void XXXOnDrawGizmos() // TODO: esto no se esta usando
    {
        var playerPosition = transform.position;
        float radius = 2;
        float sphereDistanceCenterToPlayer = 0f; // Al inicio del juego puede ser 1, pero luego para más dificultad puede ser incluso negativa.
        float sphereHeight = 0.5f; // Puede mer mejor colocarla casi a la altura del piso.
        Vector3 centerSphere = new Vector3(playerPosition.x, sphereHeight, playerPosition.z + sphereDistanceCenterToPlayer);

        var color = Color.red;
        color.a = 0.1f;
        Gizmos.color = color;
        Gizmos.DrawSphere(centerSphere, radius);

        
        // Bounding Box
        //float a = 5;
        //float b = 2;
        //float c = 5;
        //float boundsHeight = 0.5f;
        //float boundsDistanceToPlayer = 1f;
        //Vector3 center = new Vector3(playerPosition.x, boundsHeight + b / 2f, playerPosition.z + boundsDistanceToPlayer + c / 2f);
        //Gizmos.DrawCube(center, new Vector3(a, b, c));
    }
}
