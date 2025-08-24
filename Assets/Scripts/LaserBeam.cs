using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public int Damage = 2;
    public GameObject ExplosionPrefab;

    public float Duration = 2f;
    private Vector3 originalScale;
    private Vector3 growthScale = new Vector3(1f, 40f, 1f);
    private Vector3 noScale = new Vector3(1f, 0f, 1f);


    private void Start()
    {
        originalScale = transform.localScale;
    }


    public IEnumerator GrowLaserRoutine(Vector3 originalScale, Vector3 finalScale, float timeToScale)
    {
        // Scales the prefab larger to make it appear like the laser is growing when activated
        float timer = 0f;
        while (timer < timeToScale)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / timeToScale);
            transform.localScale = Vector3.Lerp(this.originalScale, growthScale, t);    // Smooths the scale from the original sized to the full size

            yield return null;
        }

        transform.localScale = finalScale;
    }


    public void GrowLaser()
    {
        StartCoroutine(GrowLaserRoutine(transform.localScale, growthScale, Duration));
    }


    public IEnumerator ShrinkLaserRoutine(Vector3 finalScale, Vector3 endScale, float timeToScale)
    {
        // Scales the prefab smaller to make it appear like the laser is shrinking when deactivated
        float timer = 0f;
        while (timer < timeToScale)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / timeToScale);
            transform.localScale = Vector3.Lerp(growthScale, noScale, t);   // Smooths the scale from the current size to look as if it is off

            yield return null;
        }

        transform.localScale = endScale;
        Destroy(gameObject);
    }


    public void ShrinkLaser()
    {
        StartCoroutine(ShrinkLaserRoutine(transform.localScale, noScale, Duration));
    }

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroids asteriod = collision.gameObject.GetComponent<Asteroids>();

        if (asteriod)
        {
            asteriod.TakeDamage(Damage);
            Instantiate(ExplosionPrefab, collision.transform.position, Quaternion.identity);
        }

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(Damage);
            Instantiate(ExplosionPrefab, collision.transform.position, Quaternion.identity);
        }
    }

}
