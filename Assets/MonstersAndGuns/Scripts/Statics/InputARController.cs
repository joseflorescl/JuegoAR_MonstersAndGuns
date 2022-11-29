using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InputARController
{
    public static bool IsTapping()
    {
#if UNITY_EDITOR      
        // Notar que se est� validando que al hacer click NO estemos sobre un elemento de la UI, como un bot�n.
        return (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) && !EventSystem.current.IsPointerOverGameObject();
#else
        return Input.touchCount > 0;
#endif
    }

}
