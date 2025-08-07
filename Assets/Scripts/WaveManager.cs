using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class EnemyOption
{
    public GameObject EnemyPrefab;
    public int Strength;
    public int UnlockWave;
}
public class WaveManager : MonoBehaviour
{
    [Header("Wave")]
    public bool WaveStarted;
    public bool WaveComplete;
    public int WaveStrength;
    public float WaveDelayTimer = 0f;
    public int CurrentWave = 0;
    public float waveDelay = 5f;
    [Header("Enemies")]
    public List<EnemyOption> AllEnemies = new List<EnemyOption>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    

    private void Start()
    {
        WaveStarted = false;
        WaveComplete = false;
    }

   
    

    public void Spawner()
    {
        int RemainingStrength = WaveStrength;
        List<EnemyOption> avaliableEnemies = AllEnemies.FindAll(e => e.UnlockWave <= CurrentWave);

        while (RemainingStrength > 0)
        {
            List<EnemyOption> validOptions = avaliableEnemies.FindAll(e => e.Strength <= RemainingStrength);

            if (validOptions.Count == 0)
                break;

            EnemyOption selected = validOptions[Random.Range(0, validOptions.Count)];
            GameObject enemy = Instantiate(selected.EnemyPrefab, transform.position, Quaternion.identity);
            spawnedEnemies.Add(enemy);

            RemainingStrength -= selected.Strength;
        }
    }

    public void StartWave()
    {
        if (WaveDelayTimer >= waveDelay)
        {
            WaveStarted = true;
            CurrentWave++;

        }

        if (WaveStarted)
        {
            //Debug.Log("Wave started");
            WaveDelayTimer = 0f;


        }
    }

    public bool AllEnemiesDefeated()
    {
        spawnedEnemies.RemoveAll(e => e == null);
        return spawnedEnemies.Count == 0;
    }

   

    private void Update()
    {
        WaveDelayTimer += Time.deltaTime;
        StartWave();
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Spawner();
        }
    }
}
