using System.Collections;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    public TMP_Text WaveBoxText;
    public GameObject WaveNumber;
    private WaveManager waveManager;
    public GameObject WaveCompleteText;
    public TMP_Text EnemiesRemaining;
    
    private void Start()
    {
        waveManager = WaveManager.instance;
        WaveCompleteText.SetActive(false);  
    }

    private void Update()
    {
        if (waveManager != null)
        {
            WaveBoxText.text = "Wave " + waveManager.CurrentWave.ToString();
            EnemiesRemaining.text = "Enemies Remaining :  " + waveManager.spawnedEnemies.Count.ToString();
        }
    }

    public void HideWaveComplete()
    {
        WaveCompleteText.SetActive(false);
    }

    public void ShowWaveComplete()
    {
        WaveCompleteText.SetActive(true);
    }

    public void HideWaveNumber()
    {
        WaveNumber.SetActive(false);
    }

    public void ShowWaveNumber()
    {
        WaveNumber.SetActive(true);
    }

    


}
