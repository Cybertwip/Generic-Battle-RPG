using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collections : MonoBehaviour
{
    public static Collections collections;
    public GameObject[] rooms;
    public List<GameObject> openDoors;

    private void Awake()
    {
        collections = this;
    }
    private void Start()
    {
        if (rooms != null) rooms = GameObject.FindGameObjectsWithTag("Room");
    }
}
