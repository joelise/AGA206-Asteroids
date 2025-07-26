using UnityEngine;
using UnityEngine.Rendering;

public class AutoDestroy : MonoBehaviour
{
    public float Lifetime = 5f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= Lifetime)
        {
            Destroy(gameObject);
        }
    }
}
