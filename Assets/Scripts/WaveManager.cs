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
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public float PushForce = 100f;
    public float Inaccuracy = 2f;

    

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

            Vector3 spawnPoint = RandomOffScreenPoint();
            spawnPoint.z = transform.position.z;

            GameObject enemy = Instantiate(selected.EnemyPrefab, spawnPoint, transform.rotation);
            Vector2 force = PushDirection(spawnPoint) * PushForce;
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            rb.AddForce(force);
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

    private Vector3 RandomOffScreenPoint()
    {
        Vector2 randomPos = Random.insideUnitCircle;
        Vector2 direction = randomPos.normalized;
        Vector2 finalPos = (Vector2)transform.position + direction * 1f;

        return Camera.main.ViewportToWorldPoint(finalPos);
    }

    private Vector2 PushDirection(Vector2 from)
    {
        Vector2 miss = Random.insideUnitCircle * Inaccuracy;
        Vector2 destination = (Vector2)transform.position + miss;
        Vector2 direction = (destination - from).normalized;

        return direction;
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
        
        if (WaveStarted && AllEnemiesDefeated())
        {
            WaveComplete = true;
            WaveStarted = false;
           
        }
    }
}
