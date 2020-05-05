using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegainFocus : MonoBehaviour
{
    GameObject lastselect;
    void Start()
    {
        lastselect = new GameObject();
    }
    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(lastselect);
        }
        else
        {
            lastselect = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        }
    }
}