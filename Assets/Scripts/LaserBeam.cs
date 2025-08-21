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
        float timer = 0f;
        while (timer < timeToScale)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / timeToScale);
            transform.localScale = Vector3.Lerp(this.originalScale, growthScale, t);

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
        float timer = 0f;
        while (timer < timeToScale)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / timeToScale);
            transform.localScale = Vector3.Lerp(growthScale, noScale, t);

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
    }

}
