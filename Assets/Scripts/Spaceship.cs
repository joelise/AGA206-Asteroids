using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public float EnginePower = 10f;
    public float TurnPower = 10f;

    [Header("Health")]
    public int HealthMax = 3;
    public int HealthCurrent;

    [Header("Bullets")]
    public GameObject BulletPreFab;
    public float BulletSpeed = 100f;
    public float FiringRate = 0.33f;
    private float fireTimer = 0f;

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


    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        HealthCurrent = HealthMax;
        HighScore = GetHighScore();
    }



    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        ApplyThrust(vertical);
        ApplyTorque(horizontal);
        UpdateFiring();
        PauseMenu();

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteKey("HighScore");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Explode();
        }
        //Debug.Log(delay);
    }

    private void UpdateFiring()
    {
        bool isFiring = Input.GetButton("Fire1");
        fireTimer -= Time.deltaTime;    //Decrement the timer

        if (isFiring && fireTimer <= 0)
        {
            FireBullet();
            fireTimer = FiringRate;
        }
    }

    private void ApplyThrust(float amount)
    {
        //Debug.Log("Thrust amount is " + amount);
        Vector2 thrust = transform.up * EnginePower * Time.deltaTime * amount;
        rb2D.AddForce(thrust);
    }


    private void ApplyTorque(float amount)
    {
        //Debug.Log("Torque amount is " + amount);
        float torque = amount * TurnPower * Time.deltaTime;
        rb2D.AddTorque(-torque);
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

        GameOver();
        Flash.Hide();
        //Time.timeScale = 0.5f;
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

        float t = 3f;
        Time.timeScale = Mathf.Lerp(1f, 0.5f, t);
    }

    public void AutoKill()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Explode();
        }
    }

    public void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = true;
            PauseUI.Show();
            Time.timeScale = 0f;
        }
        if ((PauseUI.isPaused == false) && (IsPaused = true))
        {
            IsPaused = false;
            Time.timeScale = 1f;
        }
        
    }

    
    

    
}
