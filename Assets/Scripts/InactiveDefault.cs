using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveDefault : MonoBehaviour
{

    public GameObject menu00;
    public GameObject menu01;

    void Start()
    {
        menu00.SetActive(true);
        menu01.SetActive(true);
    }
}
