using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{

    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] GameObject spawnablePrefab;
    [SerializeField] Material touchMaterial;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    GameObject spawnedObject;

    Camera arCam;

    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
        arCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0) return;

        var touchOne = Input.GetTouch(0);
        
        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(touchOne.position);

        if (touchOne.phase == TouchPhase.Began && Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Monster"))
            {
                hit.collider.gameObject.GetComponent<Renderer>().material = touchMaterial;
            }
        }


        if (raycastManager.Raycast(touchOne.position, hits))
        {
            var pose = hits[0].pose;

            if (touchOne.phase == TouchPhase.Began && spawnedObject == null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Monster"))
                    {
                        spawnedObject = hit.collider.gameObject;
                        spawnedObject.GetComponent<Renderer>().material = touchMaterial;
                    }
                    else
                        SpawnPrefab(pose.position);
                }
                
            }
            else if (touchOne.phase == TouchPhase.Moved && spawnedObject != null)
                spawnedObject.transform.position = pose.position;

            if (touchOne.phase == TouchPhase.Ended)
                spawnedObject = null;
        }
    }

    private void SpawnPrefab(Vector3 position)
    {
        spawnedObject = Instantiate(spawnablePrefab, position, Quaternion.identity);
    }
}
