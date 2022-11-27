using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxDot : MonoBehaviour
{
    public Transform player;
    public Transform point;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PrintDot();
        }
    }

    void PrintDot()
    {
        Vector3 a = player.forward;
        Vector3 b = point.position - player.position;
        a.Normalize();
        b.Normalize();
        float dot = Vector3.Dot(a, b);

        print(dot);
    }
}
