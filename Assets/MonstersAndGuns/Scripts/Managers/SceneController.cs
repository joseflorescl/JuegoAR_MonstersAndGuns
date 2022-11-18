using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public enum SceneName { Game = 0, ARSession, Menu }



    public Coroutine LoadARSessionRoutine()
    {
        return StartCoroutine( LoadSceneRoutine((int)SceneName.ARSession));
    }

    public Coroutine LoadMenu()
    {
        return StartCoroutine(LoadSceneRoutine((int)SceneName.Menu));
    }

    private IEnumerator LoadSceneRoutine(int sceneBuildIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
        yield return new WaitUntil(() => asyncLoad.isDone);
    }
    

}
