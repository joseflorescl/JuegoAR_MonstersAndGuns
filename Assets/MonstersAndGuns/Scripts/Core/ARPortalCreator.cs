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
    Camera arCamera;
    GameObject portal;
    bool isCreatingPortal;


    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arCamera = Camera.main;
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
        isCreatingPortal = true;
        arPlaneManager.enabled = true;

        while (isCreatingPortal)
        {
            Vector2 middleScreenPoint = arCamera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

            if (arRaycastManager.Raycast(middleScreenPoint, hits, TrackableType.Planes))
            {
                var pose = hits[0].pose;
                portal.transform.SetPositionAndRotation(pose.position, pose.rotation);

                SetStatusPortal(true);

                if (IsTapping())
                    isCreatingPortal = false;
            }
            else
            {
                SetStatusPortal(false);
            }

#if UNITY_EDITOR
            // Como no tengo el XR Simulator necesito probar en el editor cuando se haga click con el mouse
            if (IsTapping())
                isCreatingPortal = false;
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

    private bool IsTapping()
    {
#if UNITY_EDITOR
        if( Input.GetMouseButtonDown(0))
        {
            SetStatusPortal(true);
            return true;
        }
        return false;
#else
        return Input.touchCount > 0;
#endif
    }



}
