using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int Damage = 1;

    
    void Start()
    {
        Destroy(gameObject, 10);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        Spaceship ship = collision.gameObject.GetComponent<Spaceship>();
        if (ship != null)
        {
            ship.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }



}
