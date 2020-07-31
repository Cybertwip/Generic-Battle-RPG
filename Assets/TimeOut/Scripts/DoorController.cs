using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int id;
    float angle = 0.0f;
    float yVelocity = 0.0f;
    float smoothTime = 0f;

    public bool isOpen;
    
    void Start()
    {
        DoorEvents.current.onDoorwayTriggerEnter += OnDoorwayOpen;
        DoorEvents.current.onDoorwayTriggerExit += OnDoorwayClose;
        angle = gameObject.transform.localRotation.eulerAngles.y;
    }

    private void OnDoorwayOpen(int id)
    {
        if (id == this.id && angle == 0f) 
        {
            StartCoroutine(DoorOpen(angle, -90f, smoothTime));
        }
        else if(id != this.id)
        {
            if (this.isOpen)
            {
                StartCoroutine(DoorClose(angle, 0f, smoothTime));
            }
        }
        else if(angle != 0f)
        {
            if (this.isOpen)
            {
                StartCoroutine(DoorClose(angle, 0f, smoothTime));
            }
        }
    }

    private void OnDoorwayClose(int id)
    {
        if (id == this.id && angle == -90f) StartCoroutine(DoorClose(angle, 0f, smoothTime));
    }

    IEnumerator DoorOpen(float angle, float target, float smoothTime)
    {
        this.angle = angle;

        while (this.angle != target)/*angle < target*/ // local y-angle -90f --> 0f
        {
            smoothTime += Time.deltaTime;
            this.angle = Mathf.Ceil(Mathf.Lerp(this.angle, target, smoothTime));
            var eulerAngles = gameObject.transform.localRotation.eulerAngles;
            gameObject.transform.localRotation = Quaternion.Euler(eulerAngles.x, this.angle, eulerAngles.z);
            Debug.Log(angle + ", " + this.angle);
            yield return null;
        }
        {
            this.angle = target;

            var eulerAngles = gameObject.transform.localRotation.eulerAngles;
            gameObject.transform.localRotation = Quaternion.Euler(eulerAngles.x, this.angle, eulerAngles.z);
        }

        isOpen = true;
        yield return 0;
    }

    IEnumerator DoorClose(float angle, float target, float smoothTime)
    {
        this.angle = angle;

        while (this.angle != target)/*angle < target*/ // local y-angle -90f --> 0f
        {
            smoothTime += Time.deltaTime;

            this.angle = Mathf.Ceil(Mathf.Lerp(this.angle, target, smoothTime));
            var eulerAngles = gameObject.transform.localRotation.eulerAngles;
            gameObject.transform.localRotation = Quaternion.Euler(eulerAngles.x, this.angle, eulerAngles.z);

            yield return angle;
        }
        {
            this.angle = target;

            var eulerAngles = gameObject.transform.localRotation.eulerAngles;
            gameObject.transform.localRotation = Quaternion.Euler(eulerAngles.x, this.angle, eulerAngles.z);
        }

        isOpen = false;
        yield return angle;
    }
}
