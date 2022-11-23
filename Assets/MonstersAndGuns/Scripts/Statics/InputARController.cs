using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputARController
{
    public static bool IsTapping()
    {
#if UNITY_EDITOR      
        return Input.GetMouseButton(0);
#else
        return Input.touchCount > 0;
#endif
    }

}
