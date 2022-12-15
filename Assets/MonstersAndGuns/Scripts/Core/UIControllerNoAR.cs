using UnityEngine;

public class UIControllerNoAR : MonoBehaviour
{
    [SerializeField] private GameObject noArObjects;    
    [SerializeField] private ParticleSystem monsterDeadParticles;
    [SerializeField] private Transform explosionPoint;


    private void OnEnable()
    {
        GameManager.Instance.OnPortalCreating += PortalCreatingHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPortalCreating -= PortalCreatingHandler;
    }

    private void PortalCreatingHandler()
    {
        noArObjects.SetActive(false);
        monsterDeadParticles.transform.position = explosionPoint.position;
        monsterDeadParticles.Play();
        GameManager.Instance.OnPortalCreating -= PortalCreatingHandler;
    }
}
