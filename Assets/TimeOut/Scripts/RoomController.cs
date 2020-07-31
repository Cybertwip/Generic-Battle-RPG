using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int id;
    void Start()
    {
        DoorEvents.current.onDoorwayTriggerEnter += OnDoorwayOpen;
        DoorEvents.current.onDoorwayTriggerExit += OnDoorwayClose;
    }

    private void OnDoorwayOpen(int id)
    {
        if (id == this.id) gameObject.SetActive(true);
    }

    private void OnDoorwayClose(int id)
    {
        if (id == this.id) gameObject.SetActive(false);
    }
}
