using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStateName
{
        menu,
        platforming,
        battle,
        cutscene
}
public class GameState : MonoBehaviour
{
    private static GameState instance = null; // static (class level) variable
    public static GameState Instance { get { return instance; } } // static getter (only accessing allowed)

    public GameStateName state = GameStateName.menu;
   
    public Camera mainCamera;

    private void Awake()
    {
        // if instance is not yet set, set it and make it persistent between scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // if instance is already set and this is not the same object, destroy it
            if (this != instance) { Destroy(gameObject); }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene scene)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainCamera = GameObject.Find("/Main Camera").GetComponent<Camera>();

        mainCamera.GetComponent<CameraFade>().FadeOut();
    }

    public void FadeToScreen(string sceneName)
    {
        mainCamera = GameObject.Find("/Main Camera").GetComponent<Camera>();

        mainCamera.GetComponent<CameraFade>().OnFadeEnd = () => {
            SceneManager.LoadScene(sceneName);
        };

        mainCamera.GetComponent<CameraFade>().FadeIn();
    }
}
