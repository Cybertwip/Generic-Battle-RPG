using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SoundFX : MonoBehaviour
{

    public AudioSource CursorMoveSFX;

    // Start is called before the first frame update
    void Start()
    {
        CursorMoveSFX = GetComponent<AudioSource>();
        CursorMoveSFX.clip = Resources.Load<AudioClip>("FF7_CursorMove");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("up") || Input.GetKeyDown("down") || Input.GetKeyDown("left") || Input.GetKeyDown("right") ||
            Input.GetKeyDown("w") || Input.GetKeyDown("s") || Input.GetKeyDown("a") || Input.GetKeyDown("d"))
        {
            CursorMoveSFX.Play();
        }        
    }
}
