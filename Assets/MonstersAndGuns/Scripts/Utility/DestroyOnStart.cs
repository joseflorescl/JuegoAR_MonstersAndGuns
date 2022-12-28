using UnityEngine;

public class DestroyOnStart : MonoBehaviour
{
    void Start() => PoolManager.Instance.Release(gameObject);
}
