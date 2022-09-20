using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadSceneHere(string _SceneName)
    {
        SceneManager.LoadScene(_SceneName);
    }
}
