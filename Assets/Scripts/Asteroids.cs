using UnityEngine;

public class Asteroids : MonoBehaviour
{
    [Range(1, 3)]
    public int SpawnValue = 3;
    public int CollisionDamage = 1;
    public int HealthMax = 3;
    private int HealthCurrent;
    public bool OnScreen = false;
    public GameObject[] Chunks;
    [Header("Explosion Stuff")]
    public int ChunksMin = 0;
    public int ChunksMax = 4;
    public float ExplodeDist = 0.5f;
    public float ExplosionForce = 10f;
    [Header("Scoring")]
    public int ScoreValue = 10;

    public new SpriteRenderer renderer;

        
    private void Start()
    {
        HealthCurrent = HealthMax; 
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Spaceship ship = collision.gameObject.GetComponent<Spaceship>();
        if(ship != null)
        {
            ship.TakeDamage(CollisionDamage);
        }
        
    }

    public void TakeDamage(int damage)
    {
        if (OnScreen)
        {
            HealthCurrent -= damage;
        }
        

        if(HealthCurrent <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Spaceship ship = FindFirstObjectByType<Spaceship>();
        if (ship != null)
        {
            ship.Score += ScoreValue;
        }


        int numChunks = Random.Range(ChunksMin, ChunksMax + 1);

        for(int i = 0; i < numChunks; i++)
        {
            CreateAsteroidChunk();
        }
        //WaveManager.Instance.RemoveEnemy(gameObject);
        Destroy(gameObject);
    
    }

    public void CreateAsteroidChunk()
    {
        if (Chunks == null || Chunks.Length == 0)
            return;

        int randomIndex = Random.Range(0, Chunks.Length);

        //Find a random position to spawn it
        Vector2 spawnPos = transform.position;
        Vector2 newPos = spawnPos;
        spawnPos.x += Random.Range(-ExplodeDist, ExplodeDist);
        spawnPos.y += Random.Range(-ExplodeDist, ExplodeDist);

        GameObject chunk = Instantiate(Chunks[randomIndex], spawnPos, transform.rotation);
        WaveManager.instance.AddEnemy(chunk);
        Debug.Log("chunks added");
        Vector2 dir = (spawnPos - newPos).normalized;

        Rigidbody2D rb = chunk.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * ExplosionForce);
    }

    private void Update()
    {
        if (renderer.isVisible)
        {
            OnScreen = true;
        }
    }

}
