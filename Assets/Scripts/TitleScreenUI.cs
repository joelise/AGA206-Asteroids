using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    public SoundPlayer ClickSounds;
    public SoundPlayer BkgMusic;

    private void Start()
    {
        BkgMusic.PlaySounds();
    }
    public void ClickPlay()
    {
        ClickSounds.PlaySounds();
        SceneManager.LoadScene("Asteroids"); //The name of your gameplay sceme
    }
    

    public void ClickQuit()
    {
        ClickSounds.PlaySounds();
        Application.Quit();
    }
}
