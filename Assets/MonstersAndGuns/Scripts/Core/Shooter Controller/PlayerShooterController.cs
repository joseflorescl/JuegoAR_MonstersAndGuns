using System.Collections;
using UnityEngine;

public class PlayerShooterController : BaseShooterController
{
    [SerializeField] private int firesPerSecond = 20;    
    [SerializeField] private float maxBulletDistance = 100f; // Usada para el raycast
    [SerializeField] private LayerMask damageableLayerMask; // por ahora solo se puede disparar a los monsters    

    private void OnEnable()
    {
        GameManager.Instance.OnBattling += BattleHandler;
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;
        GameManager.Instance.OnWinLevel += WinLevelHandler;
    }
   
    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= BattleHandler;
        GameManager.Instance.OnPlayerDead -= PlayerDeadHandler;
        GameManager.Instance.OnWinLevel -= WinLevelHandler;
    }

    private void WinLevelHandler()
    {
        PutGunsDown();
    }

    private void PlayerDeadHandler()
    {
        PutGunsDown();
    }

    void PutGunsDown()
    {
        StopAllCoroutines();
        bulletFactory.gameObject.SetActive(false);
    }

    private void BattleHandler(int level)
    {
        StartCoroutine(BattleRoutine());
    }

    private void Start()
    {
        bulletFactory.gameObject.SetActive(false);
    }

    public override int FireBullet()
    {
        int gunIndex = base.FireBullet();
        GameManager.Instance.PlayerFired(gunIndex);
        return gunIndex;
    }

    private IEnumerator BattleRoutine()
    {
        bulletFactory.gameObject.SetActive(true);
        var arCamera = GameManager.Instance.ARCamera;

        float fireRate = 1f / firesPerSecond;
        float nextFire = 0f;

        while (true)
        {
            if (InputARController.IsTapping() && Time.time > nextFire)            
            {
                nextFire = Time.time + fireRate;
                FireBullet();

                Vector2 middleScreenPoint = arCamera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
                Ray ray = arCamera.ScreenPointToRay(middleScreenPoint);

                if (Physics.Raycast(ray, out RaycastHit hit, maxBulletDistance, damageableLayerMask))
                    DoDamage(hit.collider.gameObject);
            }

            yield return null;
        }
    }

}
