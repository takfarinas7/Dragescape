using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Cible à suivre")]
    public Transform target;     

    [Header("Décalage")]
    public Vector3 offset = new Vector3(0, 0, -10); 

    [Header("Lissage")]
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothed;
    }
}