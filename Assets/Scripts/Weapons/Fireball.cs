using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    float epsilon = 0f;

    void Start()
    {
        epsilon = Random.Range(-90f, 90f);
        var eulerAngles = gameObject.transform.localRotation.eulerAngles;
        gameObject.transform.localRotation = Quaternion.Euler(eulerAngles.x + epsilon, eulerAngles.y, eulerAngles.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AI_Behavior>() != null)
        {
            Destroy(this.gameObject);
        }
    }
}
