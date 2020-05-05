using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    }
}
