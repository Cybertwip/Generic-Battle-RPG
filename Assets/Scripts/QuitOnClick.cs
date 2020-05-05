using UnityEngine;
using UnityEngine.UI;

public class QuitOnClick : MonoBehaviour
{
    private Button quitButton;

    void Start()
    {
        quitButton = GetComponent<Button>();
        quitButton.onClick.AddListener(QuitGame);
    }

    private void QuitGame()
    {
        Debug.Log("Game is exiting");
        Application.Quit();
    }
}