using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public int id;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerStay(Collider player)
    {
        if (Input.GetButtonUp("Use"))
        {
            DoorEvents.current.DoorwayTriggerEnter(id);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            DoorEvents.current.DoorwayTriggerExit(id);
        }
    }
}
