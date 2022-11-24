using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooterController : ShooterController
{
    [SerializeField] private int firesPerSecond = 20;
    [SerializeField] private float waitToRaycast = 0.1f;
    [SerializeField] private float maxBulletDistance = 100f; // Usada para el raycast
    [SerializeField] private LayerMask damageableLayerMask; // por ahora solo se puede disparar a los monsters

    float fireRate;
    float nextFire;

    private void OnEnable()
    {
        GameManager.Instance.OnBattling += BattleHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= BattleHandler;
    }

    private void BattleHandler(List<MonsterController> arg1, int arg2)
    {

        StartCoroutine(BattleRoutine());
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        bulletFactory.gameObject.SetActive(false);
        fireRate = 1f / firesPerSecond;
        nextFire = 0f;
    }


    private IEnumerator BattleRoutine()
    {
        bulletFactory.gameObject.SetActive(true);
        var arCamera = GameManager.Instance.GetARCamera();

        while (true)
        {
            if (InputARController.IsTapping() && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                FireBullet();             
                
                // Ahora hay que validar si hay un monster en el medio de la pantalla, en tal caso causarle daño.
                //  Pero como el disparo se demora unos ms en llegar al centro de la pantalla, esperamos ese poquito
                yield return new WaitForSeconds(waitToRaycast);
                
                Vector2 middleScreenPoint = arCamera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
                Ray ray = arCamera.ScreenPointToRay(middleScreenPoint);

                if (Physics.Raycast(ray, out RaycastHit hit, maxBulletDistance, damageableLayerMask))
                {
                    DoDamage(hit.collider.gameObject);
                }

            }

            yield return null;
        }
    }

   
}
