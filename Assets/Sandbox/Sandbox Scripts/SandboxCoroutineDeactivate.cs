using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxCoroutineDeactivate : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(PrintRoutine());
    }

    public void StartPrintRoutine()
    {
        StartCoroutine(PrintRoutine());
    }

    private IEnumerator PrintRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            print("PrintRoutine");
        }
    }
}
