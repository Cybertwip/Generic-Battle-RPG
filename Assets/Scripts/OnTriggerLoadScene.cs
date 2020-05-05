using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//lifted from https://www.youtube.com/watch?v=tCe_UfyirT4
public class OnTriggerLoadScene : MonoBehaviour
{

    public GameObject guiObject;
    public string levelToLoad;
    public Animator animator;
    public Text text;

    void Start()
    {
        text = guiObject.GetComponentInChildren<Text>();
        text.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {

            text.text = "Press 'E' to\nFight Boss";
            text.enabled = true;
            if (Input.GetButtonDown("Use"))
            {
                StartCoroutine(FadeThenLoadScene());
            }
        }
    }

    private void OnTriggerExit()
    {
        text.text = "";
        text.enabled = false;
    }

    IEnumerator FadeThenLoadScene()
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoad);
    }
}
