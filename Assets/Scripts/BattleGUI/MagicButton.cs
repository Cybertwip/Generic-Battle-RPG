using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicButton : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject magicPanel;

    [SerializeField] GameObject[] buttons = new GameObject[4];

    public void Start()
    {
        magicPanel.SetActive(false);
    }
    public void TogglePanel()
    {
        int thisButtonIndex = gameObject.transform.GetSiblingIndex();
        Transform firstParent = gameObject.transform.parent;
        for (int i = 0; i < 4; i++)
        {
            if (i == thisButtonIndex && i == 0)
            {
                magicPanel.SetActive(!magicPanel.activeInHierarchy);
                buttons[i].transform.GetChild(2).gameObject.SetActive(false);
                for (int j = 1; j < 4; j++)
                {
                    buttons[j].transform.GetChild(2).gameObject.SetActive(false);

                }

                break;
            }
            else if(i != thisButtonIndex)
            {
                buttons[i].transform.GetChild(2).gameObject.SetActive(false);
                magicPanel.SetActive(false);
            }
            else
            {
                buttons[i].transform.GetChild(2).gameObject.SetActive(!buttons[i].transform.GetChild(2).gameObject.activeInHierarchy);
                magicPanel.SetActive(false);

            }
        }
    }

    public void CloseOnClick()
    {
        GameObject secondParent = gameObject.transform.parent.transform.gameObject;
        secondParent.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
