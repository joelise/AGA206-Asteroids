using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text ScoreBoxText;
    private Spaceship ship;
    public GameObject ScoreText;

    private void Start()
    {
        ship = FindFirstObjectByType<Spaceship>();
    }

    private void Update()
    {
        if (ship != null)
        {
            ScoreBoxText.text = "SCORE: " + ship.Score.ToString();
        }

    }

    public void Hide()
    {
        ScoreText.SetActive(false);
    }

}
