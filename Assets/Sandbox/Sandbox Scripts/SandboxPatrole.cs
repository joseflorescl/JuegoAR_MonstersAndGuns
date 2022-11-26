using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxPatrole : MonoBehaviour
{
    public MonsterData monsterData;
    public Transform player;

    bool quit;



    private void Start()
    {
        quit = false;
        StartCoroutine(SandboxPatrolRoutine());
    }


    private IEnumerator SandboxPatrolRoutine()
    {
        
        // Se calcula el centro de la esfera de patrullaje: siempre estará AL FRENTE DEL PLAYER
        Vector3 centerSphere = player.TransformPoint(Vector3.forward * monsterData.spherePatrollingDistanceCenterToPlayer);
        centerSphere.y = monsterData.spherePatrollingHeight;

        var sphere = DebugCreateCube(centerSphere, Vector3.one/2f);
        sphere.name = "Center";
        sphere.transform.rotation = player.rotation;

        while (!quit)
        {
            // Se elige un punto aleatorio en la superficie de la esfera: Debo elegir la semiesfera de arriba: 
            var targetPosition = Random.onUnitSphere * monsterData.spherePatrollingRadius;
            targetPosition.y = Mathf.Abs(targetPosition.y);
            targetPosition.z = Mathf.Abs(targetPosition.z);
            // Esta posición ahora se debe orientar c/r al centro de la esfera
            targetPosition = player.transform.TransformPoint(targetPosition) ;


            // Lo desfaso en Z
            targetPosition += player.forward * monsterData.spherePatrollingDistanceCenterToPlayer;
            // Lo desfaso en Y ( en X no lo desfaso porque siempre estará en el medio del player
            targetPosition += player.up * (monsterData.spherePatrollingHeight - player.transform.position.y);


            DebugCreateCube(targetPosition, Vector3.one * 0.1f);

            yield return new WaitForSeconds(0.1f);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            quit = true;
        }
        
    }

    GameObject DebugCreateCube(Vector3 position, Vector3 scale)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = scale;
        return cube;
    }
}
