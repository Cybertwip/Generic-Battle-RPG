using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null; // static (class level) variable
    public static AudioManager Instance { get { return instance; } } // static getter (only accessing allowed)
    
    public AudioSource bgm;
    private Scene currentScene;

    // Start is called before the first frame update
    void Awake()
    {
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
        currentScene = SceneManager.GetActiveScene();
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void ChangeBgMusic(AudioClip music)
    {
        Debug.Log("changing music");
        bgm.Stop();
        bgm.clip = music;
        bgm.Play();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded()");
        Debug.Log(scene.ToString() + ", " + mode.ToString());

        switch (scene.buildIndex)
        {
            case 1:
                Debug.Log("Case1");
                ChangeBgMusic(Resources.Load<AudioClip>("Music/And My Names Booster - Super Mario RPG"));
                break;

            case 2:
                Debug.Log("Case2");
                ChangeBgMusic(Resources.Load<AudioClip>("Music/2-18 - fight against kajidoh"));
                break;
        }
        currentScene = scene;
    }
    
}
