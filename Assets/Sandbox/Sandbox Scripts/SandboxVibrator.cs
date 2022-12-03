using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxVibrator : MonoBehaviour
{
////#if UNITY_ANDROID && !UNITY_EDITOR
//    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "Vibrator");
////#endif


//    public void Vibrate(long milliseconds = 250)
//    {
//        vibrator.Call("vibrate", milliseconds);
//        Handheld.Vibrate();
//    }

    public void VibrateHandheld()
    {
        Handheld.Vibrate();
    }

}
