using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public TMP_Text HealthText;
    private Spaceship ship;

    void Start()
    {
        ship = FindFirstObjectByType<Spaceship>();
    }

    void Update()
    {
        if (ship != null)
        {
            HealthText.text = "Health: " + ship.HealthCurrent.ToString();
        }
    }
}
