using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public bool isPaused = false;
    void Start()
    {
        Hide();
    }

    public void Hide()
    {
        PauseMenu.SetActive(false);
    }

    public void Show()
    {
        PauseMenu.SetActive(true);
        isPaused = true;
    }

    public void ClickResume()
    {
        isPaused = false;
        Hide();
    }
    public void ClickMainMenu()
    {
        SceneManager.LoadScene("Title");
    }

    public void ClickQuit()
    {
        Application.Quit();
    }
}
