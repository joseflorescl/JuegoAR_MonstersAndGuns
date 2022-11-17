using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPortalCreator : MonoBehaviour
{
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject monsterMenu;

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
        GameManager.Instance.OnPortalCreation += PortalCreationHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPortalCreation -= PortalCreationHandler;
    }


    private void PortalCreationHandler()
    {
        monsterMenu.SetActive(false);
        StartCoroutine(PortalCreationRoutine());
    }



    IEnumerator PortalCreationRoutine()
    {
        print("Ini PortalCreationRoutine");

        isCreatingPortal = true;
        arPlaneManager.enabled = true;

        while (isCreatingPortal)
        {
            Vector2 middleScreenPoint = arCamera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

            if (arRaycastManager.Raycast(middleScreenPoint, hits, TrackableType.Planes))
            {
                var pose = hits[0].pose;
                portal.transform.SetPositionAndRotation(pose.position, pose.rotation);
                portal.SetActive(true);

                if (Input.touchCount > 0)
                    isCreatingPortal = false;
            }
            else
            {
                portal.SetActive(false);
            }

            yield return null;
        }

        arPlaneManager.enabled = false;

    }
}
