using UnityEngine;

public class Enemy : MonoBehaviour
{ 
    public int CollisionDamage = 2;
    public int HealthMax = 5;
    public int HealthCurrent;
    public int ScoreValue = 50;
    public bool OnScreen = false;
    [Header("Chase Player")]
    public Transform Player;
    public float Speed = 2f;
    [Header("Shooting")]
    public bool CanShoot = false;


    public GameObject[] Powerups;
    public float SpawnChance = 0.5f;
    

    private Camera cam;
    public SpriteRenderer Renderer;

    private void Start()
    {
        HealthCurrent = HealthMax;
        cam = Camera.main;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (Renderer.isVisible)
        {
            OnScreen = true;
        }

        if (Player != null)
        {
            ChasePlayer();
        }
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

    public void ChasePlayer()
    {
        Vector2 screenSizeWorld = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float screenWidth = screenSizeWorld.x * 2f;
        float screenHeight = screenSizeWorld.y * 2f;

        Vector2 enemyPos = transform.position;
        Vector2 playerPos = Player.position;

        Vector2 rawOffset = playerPos - enemyPos;
        Vector2 offset = playerPos - enemyPos;

        if (Mathf.Abs(rawOffset.x) > screenWidth / 2f)
        {
            offset.x -= Mathf.Sign(rawOffset.x) * screenWidth;
        }

        if (Mathf.Abs(rawOffset.y) > screenHeight / 2f)
        {
            offset.y -= Mathf.Sign(rawOffset.y) * screenHeight;
        }

        Vector2 targetPos = enemyPos + offset.normalized * Speed * Time.deltaTime;
        transform.position = Vector2.Lerp(enemyPos, targetPos, 0.5f);
    }

    public void DropPowerup()
    {
        Vector2 spawnPos = transform.position;
        int randomIndex = Random.Range(0, Powerups.Length);

        GameObject powerUp = Instantiate(Powerups[randomIndex], spawnPos, transform.rotation);

    }

    public void Shoot()
    {

    }



}
