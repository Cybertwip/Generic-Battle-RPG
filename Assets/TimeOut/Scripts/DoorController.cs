using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int id;
    float angle = 0.0f;
    float yVelocity = 0.0f;
    float smoothTime = 0.3f;
    
    void Start()
    {
        DoorEvents.current.onDoorwayTriggerEnter += OnDoorwayOpen;
        DoorEvents.current.onDoorwayTriggerExit += OnDoorwayClose;
        angle = gameObject.transform.localRotation.eulerAngles.y;
    }

    private void OnDoorwayOpen(int id)
    {
        if (id == this.id && angle == 0f) StartCoroutine( DoorOpen(angle, -90f, smoothTime));
    }

    private void OnDoorwayClose(int id)
    {
        if (id == this.id && angle == -90f) StartCoroutine(DoorClose(angle, 0f, smoothTime));
    }

    IEnumerator DoorOpen(float angle, float target, float smoothTime)
    {
        while (!Mathf.Approximately(angle,target)/*angle > target*/) // local y-angle 0f --> -90f
        {
            this.angle = Mathf.SmoothDampAngle(angle, target, ref yVelocity, smoothTime);
            Debug.Log(angle + ", " + this.angle);
            yield return null;
        }
        this.angle = target;
        yield return 0;
    }

    IEnumerator DoorClose(float angle, float target, float smoothTime)
    {
        while (!Mathf.Approximately(angle, target)/*angle < target*/) // local y-angle -90f --> 0f
        {
            this.angle = Mathf.SmoothDampAngle(angle, target, ref yVelocity, smoothTime);
            yield return angle;
        }
        this.angle = target;
        yield return angle;
    }
}
