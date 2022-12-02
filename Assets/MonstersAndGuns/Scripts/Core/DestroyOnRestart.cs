using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnRestart : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnRestart += RestartHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRestart -= RestartHandler;
    }

    private void RestartHandler()
    {
        Destroy(gameObject);
    }
}
