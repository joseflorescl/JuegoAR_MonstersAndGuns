using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControllerNoAR : MonoBehaviour
{
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
        gameObject.SetActive(false);
    }
}
