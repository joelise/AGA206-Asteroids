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

[System.Serializable]
public class PowerUpOptions
{
    public GameObject PowerUpPrefab;
    public float PowerUpProbability;
}

public class WaveManager : Singleton<WaveManager>
{
    [Header("Wave")]
    public bool WaveInProgress = false;
    public int WaveStrength;
    private float strengthIncrement = 1.25f;
    public int CurrentWave = 1;
    public float WaveDelay = 5f;
    [Header("Enemies")]
    public float PushForce = 100f;
    public float Inaccuracy = 2f;
    public List<EnemyOption> AllEnemies = new List<EnemyOption>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    [Header("PowerUps")]
    public float SpawnChance = 0.2f;
    public List<PowerUpOptions> PowerUps = new List<PowerUpOptions>();
    


    private void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    public void AddEnemy(GameObject enemy)
    {
        // If the enemy is not already in the List, Add Enemy

        if (!spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Add(enemy);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }

    public bool AllEnemiesDefeated()
    {
        // Checks list amount, if list is empty AllEnemiesDefeated = true

        spawnedEnemies.RemoveAll(e => e == null); 
        return spawnedEnemies.Count == 0;

    }

    public float CalculateWaveStrength()
    {
        float startingStrength = 3f;
        return startingStrength + (CurrentWave - 1) * strengthIncrement; // Calculates the wave strength and returns a float
    }

    private Vector3 RandomOffScreenPoint()
    {
        Vector2 randomPos = Random.insideUnitCircle;
        Vector2 direction = randomPos.normalized;
        Vector2 finalPos = (Vector2)transform.position + direction * 2f;

        return Camera.main.ViewportToWorldPoint(finalPos);
    }

    private Vector2 PushDirection(Vector2 from)
    {
        Vector2 miss = Random.insideUnitCircle * Inaccuracy;
        Vector2 destination = (Vector2)transform.position + miss;
        Vector2 direction = (destination - from).normalized;

        return direction;
    }

    public void SpawnEnemies()
    {
        int RemainingStrength = WaveStrength;   // Calculates the strength of the wave
        List<EnemyOption> availableEnemies = AllEnemies.FindAll(e => e.UnlockWave <= CurrentWave);  // Finds enemies that are available to spawn at the current wave

        while (RemainingStrength > 0)   // Finds available enemies to spawn until the wave strength is reached
        {
            List<EnemyOption> validOptions = availableEnemies.FindAll(e => e.Strength <= RemainingStrength);    // Finds enemies that are available to spawn with the wave strength

            if (validOptions.Count == 0)    // If there are no enemies avaliable, stops the loop
                break;                  

            EnemyOption selected = validOptions[Random.Range(0, validOptions.Count)];   // Selects an available enemy to spawn

            // Random spawn location
            Vector3 spawnPoint = RandomOffScreenPoint();
            spawnPoint.z = transform.position.z;
            

            // Spawns and pushes the selected enemy
            GameObject enemy = Instantiate(selected.EnemyPrefab, spawnPoint, transform.rotation);
            Vector2 force = PushDirection(spawnPoint) * PushForce;
          
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            rb.AddForce(force);
            spawnedEnemies.Add(enemy);  // Adds chosen enemy to a list of spawned enemies

            RemainingStrength -= selected.Strength;     // Subtracts the strength of the chosen enemy from the wave strength, updates remaining strength
         
        }
    }

    public void SpawnPowerUp()
    {
        GameObject chosenPowerUp = RandomPowerUp();
        Vector3 spawnPoint = RandomOffScreenPoint();
        spawnPoint.z = transform.position.z;
        GameObject powerUp = Instantiate(chosenPowerUp, spawnPoint, transform.rotation);
        Vector2 force = PushDirection(spawnPoint) * 50f;
        Rigidbody2D PowerUprb = powerUp.GetComponent<Rigidbody2D>();
        PowerUprb.AddForce(force);
    }

    public IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(WaveDelay);     // Delay before wave starts

        while (true)    // Allows endless waves
        {
            WaveInProgress = true;
            WaveStrength = Mathf.RoundToInt(CalculateWaveStrength());   // Rounds the result of the wave strength to the nearest int
            SpawnEnemies();
            if (Random.value < SpawnChance)
            {
                SpawnPowerUp();
            }

            yield return new WaitUntil(() => AllEnemiesDefeated());     // Waits until all the enemies have been defeated

            CurrentWave++;      // Increments the wave number
            WaveInProgress = false;

            yield return new WaitForSeconds(WaveDelay);     // Delay before starting the next wave
        }
    }

    GameObject RandomPowerUp()
    {
        float totalProbability = 0f;

        foreach (var entry in PowerUps)
        {
            totalProbability += entry.PowerUpProbability;
        }

        float randomValue = Random.value * totalProbability;

        foreach (var entry in PowerUps)
        {
            if(randomValue < entry.PowerUpProbability)
            {
                return entry.PowerUpPrefab;
                
            }
            randomValue -= entry.PowerUpProbability;
        }

        return PowerUps[0].PowerUpPrefab;

    }

   

}
