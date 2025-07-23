using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float ShakeDuration = 0.5f;
    public float Offset = 0.1f;
 


    void Start()
    {
        StartCoroutine(ShakeRoutine());
       
    }

    public IEnumerator ShakeRoutine()
    {
        float timer = 0f;
        float t = 2f;
        Vector3 originalPos = transform.position;
        while (t < 5f)
        {
           // Vector3 originalPos = transform.position;
            Vector3 targetPos = originalPos;
            targetPos.x += Random.Range(Offset, -Offset);
            targetPos.y += Random.Range(Offset, -Offset);
            Vector3 newPos = targetPos;

            timer += Time.deltaTime;
            t = Mathf.Clamp01(timer / ShakeDuration);
            transform.position = Vector3.Lerp(originalPos, newPos, t);
            transform.position = Vector3.Lerp(newPos, originalPos, t);
            transform.position = Vector3.Lerp(originalPos, newPos, t);
            Debug.Log(timer);

            yield return new WaitForEndOfFrame();
        }
       
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //StartCoroutine(ShakeRoutine());

        
    }
}
