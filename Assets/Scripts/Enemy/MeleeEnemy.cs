using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float range = 1.5f; 
    [SerializeField] private int damage = 1;

    [Header("Detection")]
    [SerializeField] private float heightMultiplier = 1f; 
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Patrol (optionnel)")]
    [SerializeField] private MonoBehaviour enemyPatrol;

    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Health playerHealth;
    private Transform playerTransform;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
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

        Vector3 center = boxCollider.bounds.center;
        Vector3 size = new Vector3(
            boxCollider.bounds.size.x * range,
            boxCollider.bounds.size.y * heightMultiplier,
            boxCollider.bounds.size.z
        );

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, playerLayer);
        if (hits.Length == 0)
        {
            playerHealth = null;
            playerTransform = null;
            return false;
        }

        var hit = hits[0];
        playerHealth = hit.GetComponent<Health>();
        playerTransform = hit.transform;

        FaceTarget(playerTransform.position);

        return true;
    }

    private void FaceTarget(Vector3 targetPos)
    {
        float dir = Mathf.Sign(targetPos.x - transform.position.x);
        if (dir == 0f) return;

        Vector3 s = transform.localScale;
        float sign = Mathf.Sign(s.x);
        if (sign != dir)
        {
            s.x = Mathf.Abs(s.x) * dir;
            transform.localScale = s;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Vector3 center = boxCollider.bounds.center;
        Vector3 size = new Vector3(
            boxCollider.bounds.size.x * range,
            boxCollider.bounds.size.y * heightMultiplier,
            boxCollider.bounds.size.z
        );
        Gizmos.DrawWireCube(center, size);
    }

    private void DamagePlayer()
    {
        if (playerHealth != null && playerTransform != null)
        {
            if (PlayerInSight())
                playerHealth.TakeDamage(damage);
        }
    }
}
