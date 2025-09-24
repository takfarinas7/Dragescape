using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool oneShot = false;
    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        if (CheckpointManager.Instance != null)
            CheckpointManager.Instance.SetCheckpoint(transform.position);

        if (oneShot) used = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
