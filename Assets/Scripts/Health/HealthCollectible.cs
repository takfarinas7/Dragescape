using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.AddHealth(healthValue);
                gameObject.SetActive(false);
            }
        }
    }
}




