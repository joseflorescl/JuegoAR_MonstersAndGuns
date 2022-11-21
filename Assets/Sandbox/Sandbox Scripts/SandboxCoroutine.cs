using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxCoroutine : MonoBehaviour
{
    /*
     Este ej imprime lo sgte en la consola:

    1 - Start 1
    1 - A 1
    1 - B 1
    1 - A 2
    1 - Start 2
    2 - B 2
    2 - A 3

    Lo importante a notar es que cuando se llama a StartCoroutine se ejecutará el código de esa corutina hasta su primer yield


     */
    private void Start()
    {
        print(Time.frameCount + " - " +  "Start 1");
        StartCoroutine(CoroutineA());
        print(Time.frameCount + " - " + "Start 2");
    }

    private IEnumerator CoroutineA()
    {        
        print(Time.frameCount + " - " + "A 1");
        StartCoroutine(CoroutineB());
        print(Time.frameCount + " - " + "A 2");
        yield return null;
        print(Time.frameCount + " - " + "A 3");
    }

    IEnumerator CoroutineB()
    {
        print(Time.frameCount + " - " + "B 1");
        yield return null;
        print(Time.frameCount + " - " + "B 2");
    }


}
