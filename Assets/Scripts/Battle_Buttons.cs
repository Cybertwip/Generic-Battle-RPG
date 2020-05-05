using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Buttons : MonoBehaviour
{
    public void TogglePanel(GameObject thisPanel)
    {
        int thisButtonIndex = gameObject.transform.GetSiblingIndex();
        thisPanel.SetActive(!thisPanel.activeInHierarchy);
        Transform firstParent = gameObject.transform.parent;
        for (int i = 0; i < 4; i++)
        {
            if (i != thisButtonIndex)
            {
                firstParent.GetChild(i).transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public void CloseOnClick()
    {
        //On click, deactivates the panel that the button sits in, two parents up
        GameObject secondParent = gameObject.transform.parent.transform.parent.gameObject;
        secondParent.SetActive(false);
    }
}
