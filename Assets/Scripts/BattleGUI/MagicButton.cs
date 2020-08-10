using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicButton : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject magicPanel;

    public void Start()
    {
        magicPanel.SetActive(false);
    }
    public void TogglePanel()
    {
        int thisButtonIndex = gameObject.transform.GetSiblingIndex();
        magicPanel.SetActive(!magicPanel.activeInHierarchy);
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
        GameObject secondParent = gameObject.transform.parent.transform.gameObject;
        secondParent.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
