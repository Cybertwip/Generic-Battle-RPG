using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Transform target;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        //NOTE: The character's coordinate system is rotated 90 degrees clockwise (fuck)
        //so need to do adjustments here or reimport entire model
        
        Vector3 lookDirection =  target.position + 1.75f * Vector3.up - transform.position;
        float lookAngle = Vector3.Angle(-transform.right, lookDirection); // first input would normally be transform.forward
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        if (lookAngle < 80f) // doesn't work, ask around
            transform.rotation = rotation*Quaternion.Euler(0,90,0);
    }
}
