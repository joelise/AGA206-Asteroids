using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float Speed = 10f;
    public int Damage = 2;
    public GameObject ExplosionPrefab;

    public float Duration = 0.5f;
    private Vector3 OriginalScale;
    private Vector3 GrowthScale = new Vector3(1f, 30f, 1f);


    private void Start()
    {
        OriginalScale = transform.localScale;
    }

    public void ShrinkLaser()
    {
        float time = 2f;

        transform.localScale = Vector3.Lerp(GrowthScale, new Vector3(0, 0, 0), time);

        float timer = 0f;
        timer += Time.deltaTime;


        //if (timer > 3f)
        //Destroy(gameObject);
    }

    public void GrowLaser()
    {
        float time = 3f;
        //time += Time.deltaTime;
        transform.localScale = Vector3.Lerp(OriginalScale, GrowthScale, time);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroids asteriod = collision.gameObject.GetComponent<Asteroids>();

        if (asteriod)
        {
            asteriod.TakeDamage(Damage);
            Instantiate(ExplosionPrefab, collision.transform.position, Quaternion.identity);
        }
    }
}
