using UnityEngine;

public class DestroyOnRestart : MonoBehaviour
{
    private void OnEnable() => GameManager.Instance.OnRestart += RestartHandler;
    private void OnDisable() => GameManager.Instance.OnRestart -= RestartHandler;
    private void RestartHandler() => PoolManager.Instance.Release(gameObject);
}
