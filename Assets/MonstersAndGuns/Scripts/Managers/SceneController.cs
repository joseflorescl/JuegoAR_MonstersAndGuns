using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public enum SceneName { Game = 0, ARSession, Menu }
    public void LoadARSession()
    {
        SceneManager.LoadScene((int)SceneName.ARSession, LoadSceneMode.Additive);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene((int)SceneName.Menu, LoadSceneMode.Additive);
    }


}
