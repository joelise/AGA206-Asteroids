using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public SoundPlayer ClickSounds;

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
        ClickSounds.PlaySounds();
        isPaused = false;
        Hide();
    }
    public void ClickMainMenu()
    {
        ClickSounds.PlaySounds();
        SceneManager.LoadScene("Title");
    }

    public void ClickQuit()
    {
        ClickSounds.PlaySounds();
        Application.Quit();
    }






}
