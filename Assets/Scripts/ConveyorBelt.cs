using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public bool isMoving = true;
    public float speed = 0.1f;
    private float offset = 0f;
    private Vector3 targetStart;
    private Vector3 targetEnd;
    private Vector3 heading;
    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        targetStart = gameObject.transform.GetChild(0).GetChild(0).position;
        targetEnd = gameObject.transform.GetChild(0).GetChild(1).position;
        heading = targetEnd - targetStart;
        direction = heading / heading.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving == true)
        {
            MoveBelt();
        }
    }

    public void MoveBelt()
    {
        offset += (Time.deltaTime * speed);
        if (Mathf.Abs(offset) > 99)
        {
            offset %= 1f; //thought this might save memory, maybe not
        }
        gameObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }

    public void StopBelt()
    {
        isMoving = false;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody && isMoving == true)
        {
            other.transform.position = other.transform.position + direction * 10 * speed * Time.deltaTime / (0.5f * Mathf.PI + 3f);
        }
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
