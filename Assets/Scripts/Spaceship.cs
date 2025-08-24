using JetBrains.Annotations;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class Spaceship : MonoBehaviour
{
    [Header("Movement")]
    public float EnginePower = 2f;
    public float TurnPower = 200f;
    public float MaxSpeed = 3f;
    public float AngularDrag = 1f;
    public float LinearDrag = 3f;

    [Header("Health")]
    public int HealthMax = 3;
    public int HealthCurrent;

    [Header("Bullets")]
    public GameObject BulletPreFab;
    public float BulletSpeed = 100f;
    public float FiringRate = 0.33f;
    private float fireTimer = 0f;

    [Header("PowerUps")]
    public float PowerUpDuration = 10f;
    public float ScatterAngle = 30f;
    public int NumberOfBullets = 3;
    public GameObject LaserBeamPrefab;
    public Transform FirePoint;
    public GameObject activeLaser;
    public PowerUpType CurrentPowerUp;
    public GameObject ExplosionPreFab;

    [Header("Sound")]
    public SoundPlayer HitSounds;

    [Header("UI")]
    public ScreenFlash Flash;
    public GameOverUi GameOverUI;
    public ScoreUI ScoreUI;
    public PauseMenuUI PauseUI;
   
    [Header("Score")]
    public int Score;
    public int HighScore;

    [Header("Camera")]
    public CameraShake CameraShake;

    public bool IsPaused = false;

    private Rigidbody2D rb2D;
    public SpriteRenderer spriteRenderer;
    public new PolygonCollider2D collider;
    private bool isFiring;


    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.angularDamping = AngularDrag;
        rb2D.linearDamping = LinearDrag;
        HealthCurrent = HealthMax;
        HighScore = GetHighScore();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<PolygonCollider2D>();
        CurrentPowerUp = PowerUpType.Empty;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        ApplyThrust(vertical);
        ApplyTorque(horizontal);
        UpdateFiring();
        PauseMenu();

        //Delete High Score
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteKey("HighScore");
        }
    }

    private void UpdateFiring()
    {
        isFiring = Input.GetButton("Fire1");
        fireTimer -= Time.deltaTime;    //Decrement the timer

        if (!isFiring || fireTimer >= 0 || IsPaused == true)
            return;

        // Checks the powerup type and runs the function depending on the current type
        if (CurrentPowerUp != PowerUpType.ScatterShot || CurrentPowerUp != PowerUpType.LaserShot)
        {
            FireBullet();
            fireTimer = FiringRate;
        }

        if (CurrentPowerUp == PowerUpType.ScatterShot)
        {
            FireBullet();
            ScatterShot();
            fireTimer = FiringRate;
        }

        if (CurrentPowerUp == PowerUpType.LaserShot)
        {
            FireBullet();
            fireTimer = FiringRate;
        }
    }

    private void ApplyThrust(float amount)
    {
        if (amount != 0)
        {
            Vector2 thrust = transform.up * EnginePower * amount;       // Calculate thrust
            rb2D.AddForce(thrust, ForceMode2D.Force);                   // Adds force
        }

        if(rb2D.linearVelocity.magnitude > MaxSpeed)                    // If velocity is greater than max speed
        {
            // Smoothly reduce to the max speed
            rb2D.linearVelocity = Vector2.Lerp(rb2D.linearVelocity, rb2D.linearVelocity.normalized * MaxSpeed, Time.deltaTime * 2f);
        }
    }

    private void ApplyTorque(float amount)
    {
        rb2D.angularVelocity = Mathf.Lerp(rb2D.angularVelocity, -amount * TurnPower, Time.deltaTime * 2f);
        
    }

    public void TakeDamage(int damage)
    {
       
        //Reduce the current health by the damage
        HealthCurrent = HealthCurrent - damage;

        //HealthCurrent -= damage; another way for above

        HitSounds.PlayRandomSound();
        StartCoroutine(CameraShake.ShakeRoutine());
        StartCoroutine(Flash.FlashRoutine());
        
        
        if (HealthCurrent <= 0)
        {
            Explode();
        }

        //If current health is zero, then Explode
    }

    public void Explode()
    {
        //Destroy the ship, end the game
        Time.timeScale = 0.5f;
        GameOver();
        Flash.Hide();
        Destroy(gameObject);
    }

    public void FireBullet()
    {
        //Create a new bullet at the spaceships position and rotation
        GameObject bullet = Instantiate(BulletPreFab, transform.position, transform.rotation);
        //Find the bullets rigidbody component
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        //Create a force to push the bullet 'up' from the spaceship direction
        Vector2 force = transform.up * BulletSpeed;
        rb.AddForce(force);

    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    public void SetHighScore(int score)
    {
        PlayerPrefs.SetInt("HighScore", score);
    }

    public void GameOver()
    {
        bool celebrateHighScore = false;
        if(Score > GetHighScore() && celebrateHighScore == false)
        {
            SetHighScore(Score);
            celebrateHighScore = true;
        }
        ScoreUI.Hide();
        GameOverUI.Show(celebrateHighScore, Score, GetHighScore());

    }


    public void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = true;
            PauseUI.Show();
            Time.timeScale = 0f; // Pauses in game time
        }
        if ((PauseUI.isPaused == false) && (IsPaused = true))
        {
            IsPaused = false;
            Time.timeScale = 1f; // Resumes in game time
        }
        
    }

    public IEnumerator Invincibility()
    {
        collider.enabled = false;           // Turns off collision

        // Array of colours to cycle through
        Color[] rainbowColors = new Color[]
        {
            Color.red, Color.orange, Color.yellow, Color.green, Color.cyan, Color.blue, Color.purple, Color.magenta
        };

        float flashSpeed = 0.1f;
        float elapsed = 0f;
        int colorIndex = 0;

        while (elapsed < PowerUpDuration)
        {
            spriteRenderer.color = rainbowColors[colorIndex];       // Changes sprite colour
            colorIndex = (colorIndex + 1) % rainbowColors.Length;   // Cycles through the length of the colour array

            yield return new WaitForSeconds(flashSpeed);
            elapsed += flashSpeed;
        }

        CurrentPowerUp = PowerUpType.Empty;         // Resets powerup type
       
        spriteRenderer.color = Color.white;         // Returns to original colour
        collider.enabled = true;                    // turns collision on
    }

    public IEnumerator ScatterShotRoutine()
    {
        yield return new WaitForSeconds(PowerUpDuration);

        CurrentPowerUp = PowerUpType.Empty;
    }

    public void ScatterShot()
    {
        Quaternion leftRotation = transform.rotation * Quaternion.Euler(0, 0, ScatterAngle);
        Quaternion rightRotation = transform.rotation * Quaternion.Euler(0, 0, -ScatterAngle);

        // Spawns and pushes left bullet
        GameObject leftBullet = Instantiate(BulletPreFab, transform.position, leftRotation);
        Rigidbody2D rbLeft = leftBullet.GetComponent<Rigidbody2D>();
        Vector2 leftForce = leftBullet.transform.up * BulletSpeed;
        rbLeft.AddForce(leftForce);

        // Spawn and pushes right bullet
        GameObject rightBullet = Instantiate(BulletPreFab, transform.position, rightRotation);
        Rigidbody2D rbRight = rightBullet.GetComponent<Rigidbody2D>();
        Vector2 rightForce = rightBullet.transform.up * BulletSpeed;
        rbRight.AddForce(rightForce);
       
    }

    public IEnumerator LaserShotRoutine()
    {
        // Spawns laser prefab and scales it larger
        activeLaser = Instantiate(LaserBeamPrefab, FirePoint.position, transform.rotation, transform);
        activeLaser.GetComponent<LaserBeam>().GrowLaser();

        yield return new WaitForSeconds(PowerUpDuration);
        // Resets powerup type
        CurrentPowerUp = PowerUpType.Empty;
        // Shrinks the laser
        activeLaser.GetComponent<LaserBeam>().ShrinkLaser();
    }

    public IEnumerator ClearWaveRoutine()
    {
        foreach (GameObject spawnedEnemies in WaveManager.instance.spawnedEnemies)
        {
            if(spawnedEnemies != null)      // If there are enemys spawned
            {
                Rigidbody2D enemyRb = spawnedEnemies.GetComponent<Rigidbody2D>();

                if (enemyRb != null)
                {
                    Vector2 worldCenter = Vector2.zero;     // Gets the center of the world

                    enemyRb.AddForce(worldCenter * 5f, ForceMode2D.Impulse);    // Pushes enemys away from the center
                    
                }
            }
        }


        yield return new WaitForSeconds(0.3f);


        foreach (GameObject spawnedEnemies in WaveManager.instance.spawnedEnemies)
        {
            // For each enemy spawn an explosion and destroy the enemy
            Instantiate(ExplosionPreFab, spawnedEnemies.transform.position, spawnedEnemies.transform.rotation);
            Destroy(spawnedEnemies);
        }

        CurrentPowerUp = PowerUpType.Empty;     // resets the powerup type
    }

    public void FullHealth()
    {
        HealthCurrent = HealthMax;
        CurrentPowerUp = PowerUpType.Empty;
    }

    public void ApplyPowerUp(PowerUpType powerUp)
    {
        // Runs each function depending on what powerup was collected
        CurrentPowerUp = powerUp;
        switch (powerUp)
        {
            case PowerUpType.Invincibility:
                StartCoroutine(Invincibility());
                break;

            case PowerUpType.ScatterShot:
                StartCoroutine(ScatterShotRoutine());
                break;

            case PowerUpType.LaserShot:
                StartCoroutine(LaserShotRoutine());
                break;

            case PowerUpType.ClearWave:
                StartCoroutine(ClearWaveRoutine());
                break;

            case PowerUpType.FullHealth:
                FullHealth();
                break;

                
        }
    }
}
