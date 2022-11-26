using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxPatrole : MonoBehaviour
{
    public MonsterData monsterData;
    public int cubeCount = 200;
    public float sizeCubes = 0.05f;

    private void Start()
    {
        StartCoroutine(DebugCreateCubesRoutine());
    }


    GameObject DebugCreateCube(Vector3 position, Vector3 scale)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = scale;
        return cube;
    }

    IEnumerator DebugCreateCubesRoutine()
    {
        yield return new WaitForSeconds(30); // Esperamos hasta que el portal este creado

        var portal = GameManager.Instance.Portal();
        var offset = new Vector3(0f, monsterData.spherePatrollingHeightToPortal, monsterData.spherePatrollingDistanceToPortal);

        for (int i = 0; i < cubeCount; i++)
        {
            // Se elige un punto aleatorio en la superficie de la esfera de radio r:
            var targetPosition = Random.onUnitSphere * monsterData.spherePatrollingRadius;
            // Debo elegir la semiesfera de arriba/adelante
            targetPosition.y = Mathf.Abs(targetPosition.y);
            targetPosition.z = Mathf.Abs(targetPosition.z);
            // Y se aplica el offset, por ahora con respecto al origen 0,0,0
            targetPosition += offset;
            // Esta posición ahora se debe orientar c/r al player
            targetPosition = portal.transform.TransformPoint(targetPosition);

            DebugCreateCube(targetPosition, Vector3.one * sizeCubes);

            yield return null;
        }
    }
}
