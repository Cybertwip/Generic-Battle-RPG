using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Battle_ActionPanel : MonoBehaviour
{
    Text[] panelText;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<Text>().text = "THWOMP";
        transform.GetChild(1).GetComponent<Text>().text = "Jump";
    }
}
