using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSandboxCoroutineDeactivate : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var obj = Instantiate(prefab, transform.position, transform.rotation);
            obj.GetComponent<SandboxCoroutineDeactivate>().StartPrintRoutine();
        }
    }
}
