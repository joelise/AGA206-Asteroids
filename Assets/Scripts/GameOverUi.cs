using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUi : MonoBehaviour
{
    public TMP_Text ScoreTextBox, HighScoreTextBox;
    public GameObject GameOverPanel;
    public GameObject Celebrate;

    void Start()
    {
        Hide();
        Celebrate.SetActive(false);
    }

    public void Hide()
    {
        GameOverPanel.SetActive(false);
    }

    public void Show(bool celebrateHighScore, int score, int highscore)
    {
        ScoreTextBox.text = score.ToString();
        HighScoreTextBox.text = highscore.ToString();
        GameOverPanel.SetActive(true);
        Celebrate.SetActive(celebrateHighScore);
    }

    public void ClickPlayAgain()
    {
        SceneManager.LoadScene("Asteroids");
    }

    public void ClickMainMenu() 
    {
        SceneManager.LoadScene("Title");
    } 
}
