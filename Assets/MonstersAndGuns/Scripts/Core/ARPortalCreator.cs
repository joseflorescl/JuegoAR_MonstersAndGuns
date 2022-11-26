using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPortalCreator : MonoBehaviour
{
    [SerializeField] private GameObject portalPrefab;

    ARRaycastManager arRaycastManager;
    ARPlaneManager arPlaneManager;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    GameObject portal;
    bool isCreatingPortal;


    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
    }

    private void Start()
    {
        portal = Instantiate(portalPrefab);
        portal.SetActive(false);
        arPlaneManager.enabled = false;
        isCreatingPortal = false;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPortalCreating += PortalCreationHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPortalCreating -= PortalCreationHandler;
    }


    private void PortalCreationHandler()
    {
        //monsterMenu.SetActive(false);
        SetStatusPortal(false);
        StartCoroutine(PortalCreationRoutine());
    }


    IEnumerator PortalCreationRoutine()
    {
        // TODO: setear el valor del fov de la camara en el rango: 76 - 107
        // TODO: ver tutoriales de minimap
        isCreatingPortal = true;
        arPlaneManager.enabled = true;
        var arCamera = GameManager.Instance.GetARCamera();

        while (isCreatingPortal)
        {
            Vector2 middleScreenPoint = arCamera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

            if (arRaycastManager.Raycast(middleScreenPoint, hits, TrackableType.Planes))
            {
                var pose = hits[0].pose;
                portal.transform.SetPositionAndRotation(pose.position, pose.rotation);

                SetStatusPortal(true);

                if (InputARController.IsTapping())
                    isCreatingPortal = false;
            }
            else
            {
                SetStatusPortal(false);
            }

#if UNITY_EDITOR
            // Como no tengo el XR Simulator necesito probar en el editor cuando se haga click con el mouse
            if (InputARController.IsTapping()) 
            {
                SetStatusPortal(true);
                isCreatingPortal = false;
            }
#endif

            yield return null;
        }

        arPlaneManager.enabled = false;

        GameManager.Instance.PortalCreated(portal.transform);
    }
    

    private void SetStatusPortal(bool status)
    {
        portal.SetActive(status);
        GameManager.Instance.StatusPortal(status);
    }

}
