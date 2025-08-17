using System.Collections;
using UnityEngine;

public enum PowerUpType
{
    LaserShot,
    Invincibility,
    ScatterShot,
    ClearWave
}

public class PowerUps : MonoBehaviour
{
    public PowerUpType PowerUp;
    public Spaceship Ship;
    public float Duration = 10f;
    public bool HasPowerUp;

    public void Invincibility()
    {
        PowerUp = PowerUpType.Invincibility;
        Ship.GetComponent<PolygonCollider2D>().enabled = false;
        StartCoroutine(PowerUpTimer());
        Ship.GetComponent<PolygonCollider2D>().enabled = true;
    }

    public IEnumerator PowerUpTimer()
    {
        if (HasPowerUp)
            yield return new WaitForSeconds(Duration);

        HasPowerUp = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Spaceship ship = collision.gameObject.GetComponent<Spaceship>();
        if (ship != null)
        {
            HasPowerUp = true;
            if (PowerUp == PowerUpType.Invincibility)
                Invincibility();
        }
    }
   
}
