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
    [SerializeField]
    public PowerUpType powerUpType;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Spaceship ship = collision.gameObject.GetComponent<Spaceship>();
        if (ship != null)
        {
            ship.ApplyPowerUp(powerUpType);
            Destroy(gameObject);
        }
       
    }
   
}
