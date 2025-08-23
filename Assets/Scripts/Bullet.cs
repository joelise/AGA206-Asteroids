using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage = 1;
    public GameObject ExplosionPrefab;

    private void Start()
    {
        Destroy(gameObject, 10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroids asteriod = collision.gameObject.GetComponent<Asteroids>();
        if (asteriod)
        {
            asteriod.TakeDamage(Damage);
            Explode();
        }

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(Damage);
            Explode();
        }

        
    }

    private void Explode()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
