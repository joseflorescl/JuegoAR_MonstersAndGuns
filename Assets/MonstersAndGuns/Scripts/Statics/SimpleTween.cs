using System.Collections;
using UnityEngine;

public static class SimpleTween
{
    public static IEnumerator TweenRoutine(float startValue, float targetValue, float duration, 
        System.Action<float> tween, 
        System.Action postTween)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float value = Mathf.Lerp(startValue, targetValue, timeElapsed / duration);
            tween(value);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        postTween();        
    }
}
