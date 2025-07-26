using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] AsteroidRefs;   // Asteroids to spawn
    public float CheckInterval = 3f;    // The interval of time to check if we can spawn
    public float PushForce = 100f;      // The force to push the asteroids
    public int SpawnThreshold = 10;     // the limit of asteroids we can spawn

    public float Inaccuracy = 2f;

    private float checkTimer = 0f;


    private void Update()
    {
        checkTimer += Time.deltaTime;
        if(checkTimer > CheckInterval)
        {
            checkTimer = 0f;

            if(TotalAsteroidValue() < SpawnThreshold)
            {
                SpawnAsteroid();
            }
        }
    }

    private void SpawnAsteroid()
    {
        //pick an asteroid to spawn
        int asteroidIndex = Random.Range(0, AsteroidRefs.Length);
        GameObject asteroidRef = AsteroidRefs[asteroidIndex];

        // Find a random spawn point
        Vector3 spawnPoint = RandomOffScreenPoint();
        spawnPoint.z = transform.position.z;

        // Spawn the asteroid
        GameObject asteroid = Instantiate(asteroidRef, spawnPoint, transform.rotation);

        // Push the asteroid
        Vector2 force = PushDirection(spawnPoint) * PushForce;
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        rb.AddForce(force);
       
    }

    private Vector3 RandomOffScreenPoint()
    {
        Vector2 randomPos = Random.insideUnitCircle;        // Finds off screen point
        Vector2 direction = randomPos.normalized;           // Normalized to get direction
        Vector2 finalPos = (Vector2)transform.position + direction * 2f;

        return Camera.main.ViewportToWorldPoint(finalPos);  // Converts to World Space
    }
    public int TotalAsteroidValue()
    {
        //Calculates the total value of asteroids in the scene
        Asteroids[] asteroids = FindObjectsByType<Asteroids>(FindObjectsSortMode.None);
        int value = 0;
        for(int i = 0; i < asteroids.Length; i++)
        {
            value += asteroids[i].SpawnValue;
        }
        return value;
    }

    private Vector2 PushDirection(Vector2 from)
    {
        Vector2 miss = Random.insideUnitCircle * Inaccuracy;
        Vector2 destination = (Vector2)transform.position + miss;
        Vector2 direction = (destination - from).normalized;

        return direction;
    }
}
