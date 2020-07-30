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
    //string doorSuffix = "";
    // Start is called before the first frame update
    void Start()
    {
        DoorEvents.current.onDoorwayTriggerEnter += OnDoorwayOpen;
        DoorEvents.current.onDoorwayTriggerExit += OnDoorwayClose;
        angle = gameObject.transform.localRotation.y;
        //doorSuffix = gameObject.name.Substring(gameObject.name.Length - 3);
    }

    private void OnDoorwayOpen(int id)
    {
        if (id == this.id && angle == 0f) StartCoroutine(DoorOpen(angle, -90f, 0.3f));
    }

    private void OnDoorwayClose(int id)
    {
        if (id == this.id && angle == -90f) StartCoroutine(DoorClose(angle, 0f, 0.3f));
    }

    IEnumerator DoorOpen(float angle, float target, float smoothTime)
    {
        while (angle > target) // local y-angle 0f --> -90f
        {
            angle = Mathf.SmoothDampAngle(angle, target, ref yVelocity, smoothTime);
            yield return null;
        }
        angle = target;
        yield return 0;
    }

    IEnumerator DoorClose(float angle, float target, float smoothTime)
    {
        while (angle < target) // local y-angle -90f --> 0f
        {
            angle = Mathf.SmoothDampAngle(angle, target, ref yVelocity, smoothTime);
            yield return null;
        }
        angle = target;
        yield return 0;
    }
}
