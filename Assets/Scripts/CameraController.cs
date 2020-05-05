using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    private void FreeCameraFollow(Transform target)
    {
        // use this to follow character in x, y, z:
        Vector3 targetPos = target.position;
        transform.position = new Vector3(targetPos.x + 10f, targetPos.y + 8.571067812f, targetPos.z - 10f);
    }

    private void VerticalCameraFollow(Transform target)
    {
        // use this to follow character only in y:
        Vector3 targetPos = target.position;
        transform.position = new Vector3(10f, targetPos.y + 8.571067812f - 0.5f * target.position.x + 0.5f * target.position.z, -10f);
    }

    void Update()
    {
        VerticalCameraFollow(target);
    }
}
