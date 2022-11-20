using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //public enum SceneName { Game = 0, ARSession, Menu }

    //public Coroutine LoadARSessionRoutine()
    //{
    //    return StartCoroutine( LoadSceneRoutine((int)SceneName.ARSession));
    //}

    //public Coroutine LoadMenu()
    //{
    //    return StartCoroutine(LoadSceneRoutine((int)SceneName.Menu));
    //}

    public Coroutine LoadSceneAdditive(string sceneName)
    {
        return StartCoroutine(LoadSceneRoutine(sceneName));
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => asyncLoad.isDone);
    }
    

}
