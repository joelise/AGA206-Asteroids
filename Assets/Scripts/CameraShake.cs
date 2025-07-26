using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float ShakeDuration = 0.5f;
    public float Offset = 0.1f; // Distance able to move from the original Posistion


    public IEnumerator ShakeRoutine()
    {
        float timer = 0f;
        float t = 2f;
        Vector3 originalPos = transform.position;

        while (t < 5f)
        {
            // Gets Random Position
            Vector3 targetPos = originalPos;
            targetPos.x += Random.Range(Offset, -Offset);
            targetPos.y += Random.Range(Offset, -Offset);
            Vector3 newPos = targetPos;

            // Moves Camera Position
            timer += Time.deltaTime;
            t = Mathf.Clamp01(timer / ShakeDuration);
            transform.position = Vector3.Lerp(originalPos, newPos, t);
            transform.position = Vector3.Lerp(newPos, originalPos, t);

            yield return new WaitForEndOfFrame();
        }
       
    }
}
