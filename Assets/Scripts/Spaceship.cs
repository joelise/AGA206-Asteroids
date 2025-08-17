using JetBrains.Annotations;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [Header("Movement")]
    public float EnginePower = 2f;
    public float TurnPower = 200f;
    public float MaxSpeed = 3f;
    public float AngularDrag = 1f;
    public float LinearDrag = 3f;
    //public float AccelSmoothing = 2f;
    //private float smoothVertical;
    //private float currentThrust;

    [Header("Health")]
    public int HealthMax = 3;
    public int HealthCurrent;

    [Header("Bullets")]
    public GameObject BulletPreFab;
    public float BulletSpeed = 100f;
    public float FiringRate = 0.33f;
    private float fireTimer = 0f;

    [Header("PowerUps")]
    public bool HasPowerUp = false;
    public float PowerUpDuration = 10f;
    public float ScatterAngle = 15f;
    public bool ScatterShotActive = false;

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

        
        if (isFiring && fireTimer <= 0 && IsPaused == false)
        {
            FireBullet();
            fireTimer = FiringRate;
        }
    }

    private void ApplyThrust(float amount)
    {
        //smoothVertical = Mathf.Lerp(smoothVertical, amount * EnginePower, Time.deltaTime * AccelSmoothing);
        //currentThrust = Mathf.Lerp(currentThrust, smoothVertical * EnginePower, Time.deltaTime * AccelSmoothing);

        if (amount != 0)
        {
            Vector2 thrust = transform.up * EnginePower * amount;
            rb2D.AddForce(thrust, ForceMode2D.Force);
        }

        //rb2D.AddForce(transform.up * currentThrust, ForceMode2D.Force);
        
        if(rb2D.linearVelocity.magnitude > MaxSpeed)
        {
            rb2D.linearVelocity = Vector2.Lerp(rb2D.linearVelocity, rb2D.linearVelocity.normalized * MaxSpeed, Time.deltaTime * 2f);
        }
    }


    private void ApplyTorque(float amount)
    {
        rb2D.angularVelocity = Mathf.Lerp(rb2D.angularVelocity, -amount * TurnPower, Time.deltaTime * 2f);
        //Debug.Log("Torque amount is " + amount);
       // float torque = amount * TurnPower * Time.deltaTime;
        //rb2D.AddTorque(-torque);
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
        //Debug.Log("Game Over");
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
        collider.enabled = false;
        HasPowerUp = true;

        Color[] rainbowColors = new Color[]
        {
            Color.red, Color.orange, Color.yellow, Color.green, Color.cyan, Color.blue, Color.purple, Color.magenta
        };

        float flashSpeed = 0.1f;
        float elapsed = 0f;
        int colorIndex = 0;

        while (elapsed < PowerUpDuration)
        {
            spriteRenderer.color = rainbowColors[colorIndex];
            colorIndex = (colorIndex + 1) % rainbowColors.Length;

            yield return new WaitForSeconds(flashSpeed);
            elapsed += flashSpeed;
        }
       
        spriteRenderer.color = Color.white;
        collider.enabled = true;
        HasPowerUp = false;
    }

    public IEnumerator ScatterShot()
    {
        ScatterShotActive = true;
        Quaternion leftRotation = transform.rotation * Quaternion.Euler(0, 0, ScatterAngle);
        Quaternion rightRotation = transform.rotation * Quaternion.Euler(0, 0, -ScatterAngle);

        if (Input.GetButtonDown("Fire1") && isFiring)
        {
            ScatterShotActive = Input.GetButtonDown("Fire1");
            GameObject leftBullet = Instantiate(BulletPreFab, transform.position, leftRotation);
            Rigidbody2D rbLeft = leftBullet.GetComponent<Rigidbody2D>();
            Vector2 force = transform.up * BulletSpeed;
            rbLeft.AddForce(force);

            
            GameObject rightBullet = Instantiate(BulletPreFab, transform.position, rightRotation);
            Rigidbody2D rbRight = rightBullet.GetComponent<Rigidbody2D>();
            //Vector2 force = transform.up * BulletSpeed;
            rbRight.AddForce(force);
        }

        yield return new WaitForSeconds(PowerUpDuration);
        ScatterShotActive = false;
        HasPowerUp = false;
    }

    

    public void ApplyPowerUp(PowerUpType powerUp)
    {
        switch (powerUp)
        {
            case PowerUpType.Invincibility:
                Debug.Log("Invincibility active");
                StartCoroutine(Invincibility());
                break;

            case PowerUpType.ScatterShot:
                Debug.Log("ScatterShot active");
                StartCoroutine(ScatterShot());
                break;

                
        }
    }
}
