using System.Collections;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    [SerializeField] private float delayVibration = 0.1f;

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerDamage += PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerDamage -= PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead -= PlayerDeadHandler;
    }

    private void PlayerDamageHandler(float obj) => Vibrate();
    
    private void PlayerDeadHandler() => Vibrate();

    void Vibrate()
    {
        StartCoroutine(VibrateCoroutine());
    }

    IEnumerator VibrateCoroutine()
    {
        yield return new WaitForSeconds(delayVibration);
#if !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }
}
