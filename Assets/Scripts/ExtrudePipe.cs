using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrudePipe : MonoBehaviour
{
    public float pipeHeight = 1.0f;
    

    void OnValidate()
    {
        Extrusion();
    }

    void OnAwake()
    {
        Extrusion();
    }

    void Extrusion()
    {
        Transform neck = this.gameObject.transform.GetChild(1);
        Transform cap = this.gameObject.transform.GetChild(0);
        neck.transform.localScale = Vector3.one;
        cap.transform.position = Vector3.zero;

        if (pipeHeight < 1)
        {
            pipeHeight = 1;
            return;
        }
        
        float scaleFactor = (pipeHeight -0.5f)/ (0.5f * neck.transform.localScale.y);
        neck.transform.localScale = (pipeHeight > 1) ? new Vector3(1, scaleFactor, 1) : Vector3.one;
        cap.transform.localPosition = (pipeHeight > 1) ? new Vector3(0, pipeHeight - 1, 0) : Vector3.zero;
    }
}
