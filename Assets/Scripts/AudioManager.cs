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

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            if (this != instance) { Destroy(gameObject); }
        }
        currentScene = SceneManager.GetActiveScene();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void ChangeBgMusic(AudioClip music)
    {
        bgm.Stop();
        bgm.clip = music;
        bgm.Play();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 1:
                ChangeBgMusic(Resources.Load<AudioClip>("Music/And My Names Booster - Super Mario RPG"));
                break;

            case 2:
                ChangeBgMusic(Resources.Load<AudioClip>("Music/2-18 - fight against kajidoh"));
                break;
        }
        currentScene = scene;
    }
    
}
