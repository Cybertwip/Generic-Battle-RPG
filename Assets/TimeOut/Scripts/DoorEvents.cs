using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEvents : MonoBehaviour
{
    //public GameObject luigi;
    //public GameObject coa_prefab;
    //private GameObject coa;
    //private float stopwatch;

    public static DoorEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int> onDoorwayTriggerEnter;
    public void DoorwayTriggerEnter(int id)
    {
        if (onDoorwayTriggerEnter != null) onDoorwayTriggerEnter(id);
    }

    public event Action<int> onDoorwayTriggerExit;
    public void DoorwayTriggerExit(int id)
    {
        if (onDoorwayTriggerExit != null) onDoorwayTriggerExit(id);
    }
    /*
    void Update()
    {
        if (Input.GetButtonUp("Use"))
        {
            StartCoroutine(SummonSymbol());
        }
    }

    
    IEnumerator SummonSymbol()
    {
        if (coa == null)
        {
            coa = Instantiate(coa_prefab,
                            luigi.transform.position + new Vector3(0, 3.5f, 0),
                            Camera.main.transform.rotation);
        }
        while (stopwatch < 2f)
        {
            coa.GetComponentInChildren<MeshRenderer>().material.SetFloat("Vector1_DEEF3BD2", Mathf.Abs(Mathf.Sin(stopwatch * Mathf.PI / 2.0f)));
            stopwatch += Time.deltaTime;
            Debug.Log(stopwatch);
            yield return null;
        }
        if (stopwatch >= 2f)
        {
            Destroy(coa);
            stopwatch = 0f;
        }
    }
    */
}