using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float range = 1f;
    [SerializeField] private int damage = 1;

    [Header("Detection")]
    [SerializeField] private float colliderDistance = 1f;
    [SerializeField] private BoxCollider2D boxCollider; 
    [SerializeField] private LayerMask playerLayer;

    [Header("Patrol (optionnel)")]
    [SerializeField] private MonoBehaviour enemyPatrol; 
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Health playerHealth;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        bool inSight = PlayerInSight();

        if (inSight && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0f;
            anim.SetTrigger("meleeAttack");
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !inSight;
    }

    private bool PlayerInSight()
    {
        if (boxCollider == null) return false;

        Vector3 dir = transform.right * transform.localScale.x;
        Vector3 center = boxCollider.bounds.center + dir * range * colliderDistance;
        Vector3 size = new Vector3(boxCollider.bounds.size.x * range,
                                   boxCollider.bounds.size.y,
                                   boxCollider.bounds.size.z);

        RaycastHit2D hit = Physics2D.BoxCast(center, size, 0f, Vector2.left, 0f, playerLayer);

        if (hit.collider != null)
            playerHealth = hit.transform.GetComponent<Health>();

        return hit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null) return;

        Gizmos.color = Color.red;

        Vector3 dir = transform.right * transform.localScale.x;
        Vector3 center = boxCollider.bounds.center + dir * range * colliderDistance;
        Vector3 size = new Vector3(boxCollider.bounds.size.x * range,
                                   boxCollider.bounds.size.y,
                                   boxCollider.bounds.size.z);

        Gizmos.DrawWireCube(center, size);
    }

    private void DamagePlayer()
    {
        if (playerHealth != null && PlayerInSight())
            playerHealth.TakeDamage(damage);
    }
}
