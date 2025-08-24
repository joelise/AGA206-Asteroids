using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Damage & Health")]
    public int CollisionDamage = 2;
    public int HealthMax = 5;
    public int HealthCurrent;
    public bool OnScreen = false;

    [Header("Chase Player")]
    public Transform Player;
    public float Speed = 2f;
  
    [Header("Shooting")]
    public bool CanShoot;
    public GameObject BulletPreFab;
    public float BulletSpeed = 100f;
    public float FiringRate = 1f;
    private float fireTimer = 0f;

    [Header("PowerUps")]
    public GameObject[] Powerups;
    public float SpawnChance = 0.5f;

    [Header("Scoring")]
    public int ScoreValue = 50;

    [Header("Components")]
    public SpriteRenderer Renderer;
    Rigidbody2D Rb;
    Camera Cam;
   
    private void Start()
    {
        HealthCurrent = HealthMax;
        Rb = GetComponent<Rigidbody2D>();
        Cam = Camera.main;

        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }

    private void Update()
    {
        if (Renderer.isVisible)
        {
            OnScreen = true;  
        }

        if (CanShoot)
        {
            UpdateFiring();
        }
    }

    private void FixedUpdate()
    {
        ChasePlayer();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Spaceship ship = collision.gameObject.GetComponent<Spaceship>();
        if (ship != null)
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

        if (HealthCurrent <= 0)
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

        if(Random.value < SpawnChance)
        {
            DropPowerup();
        }

        Destroy(gameObject);
    }

    private Vector2 FindPlayerDirection()
    {
        if (Player == null)
            return Vector2.zero;    // If there is no player return no direction


        // Converts screen size into world units from the centre
        Vector2 screenSizeWorld = Cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // Calculates the full width and height by doubling
        float screenWidth = screenSizeWorld.x * 2f;
        float screenHeight = screenSizeWorld.y * 2f;

        // Direction from enemy to player
        Vector2 offset = Player.position - transform.position;

        // Handles player position with screen wrap so the enemy still follows the player through the screen wrap
        if (Mathf.Abs(offset.x) > screenWidth / 2f)             // If the distance along X is greater than half the screen width
        {
            // Mathf.Sign(offset.x) returns positive or negative number so the enemy knows if the player is to the right or left
            offset.x -= Mathf.Sign(offset.x) * screenWidth;     // Flips the direction so the enemy goes through the screen wrap          
        }

        if (Mathf.Abs(offset.y) > screenHeight / 2f)            // If the distance along Y is greater than half the screen height
        {
            // Mathf.Sign(offset.y) returns positive or negative number so the enemy knows if the player is above or below
            offset.y -= Mathf.Sign(offset.y) * screenHeight;    // Flips the direction so the enemy goes through the screen wrap 
        }

        return offset.normalized;
    }

    public void ChasePlayer()
    {
        if (Player == null)
            return;

        Vector2 enemyPos = transform.position;
        Vector2 direction = FindPlayerDirection();

        Vector2 targetPos = enemyPos + direction * Speed * Time.deltaTime;
        Vector2 smoothPos = Vector2.Lerp(enemyPos, targetPos, 0.5f);

        Rb.MovePosition(smoothPos);     // Moves enemy in the direction of the player
        
    }

    
    public void DropPowerup()
    {
        Vector2 spawnPos = transform.position;                  // Gets location to spawn
        int randomIndex = Random.Range(0, Powerups.Length);     // Gets random powerup

        GameObject powerUp = Instantiate(Powerups[randomIndex], spawnPos, transform.rotation);  // Spawns powerup

    }

    public void ShootAtPlayer()
    {
        if (BulletPreFab == null || Player == null)
            return;

        Vector2 direction = FindPlayerDirection();

        // Spawns bullet Prefab
        GameObject bullet = Instantiate(BulletPreFab, transform.position, Quaternion.identity);
        // Find the bullets rigidbody component
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        // Finds the Player position from the right of the enemy
        bullet.transform.right = direction;
        // Create a force to push the bullet towards the player
        Vector2 force = direction * BulletSpeed;
        rb.AddForce(force);
    }

    public void UpdateFiring()
    {
        if (fireTimer <= 0f)
        {
            ShootAtPlayer();
            fireTimer = 1f / FiringRate;
        }
        fireTimer -= Time.deltaTime;
    }
}
