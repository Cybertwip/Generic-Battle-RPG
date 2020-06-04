using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Transform target;
	private Quaternion previous_rotation;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
	void Update()
	{
		previous_rotation = transform.rotation;
        Vector3 lookDirection =  target.position + 1.75f * Vector3.up - transform.position;
        float lookAngle = Vector3.Angle(-Vector3.forward, lookDirection);
		//Debug.Log(lookAngle + "    " + lookDirection);
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        if (Mathf.Abs(lookAngle) < 80f)
		{
            transform.rotation = rotation;
		}
		else transform.rotation = previous_rotation;
    }
}
