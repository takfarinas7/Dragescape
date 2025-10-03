using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Points de patrouille")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Référence Ennemi")]
    [SerializeField] private Transform enemy;

    [Header("Mouvement (Patrol)")]
    [SerializeField] private float speed = 2f;   
    private Vector3 initScale;
    private bool movingLeft;

    [Header("Idle entre A et B")]
    [SerializeField] private float idleDuration = 1f;
    private float idleTimer;

    [Header("Animator (facultatif)")]
    [SerializeField] private Animator anim; 

    [Header("Poursuite (Chase)")]
    [SerializeField] private Transform player;        
    [SerializeField] private float detectionRadius = 6f;
    [SerializeField] private float chaseSpeed = 3.2f;    
    [SerializeField] private float stopDistance = 0.6f; 
    [SerializeField] private bool lockWithinBounds = false; 

    private void Awake()
    {
        if (enemy == null) enemy = transform;
        initScale = enemy.localScale;

        if (anim == null) anim = enemy.GetComponent<Animator>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        if (anim != null) anim.SetBool("moving", false);
        if (anim != null && anim.HasParameter("isChasing")) anim.SetBool("isChasing", false);
    }

    private void Update()
    {
        if (leftEdge == null || rightEdge == null) return;

        if (PlayerInRange())
        {
            Chase();
        }
        else
        {
            PatrolUpdate();
        }
    }

    private void PatrolUpdate()
    {
        if (anim != null && anim.HasParameter("isChasing")) anim.SetBool("isChasing", false);

        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1, speed);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1, speed);
            else
                DirectionChange();
        }
    }

    private void DirectionChange()
    {
        if (anim != null) anim.SetBool("moving", false);
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0f;
        }
    }

    private void MoveInDirection(int dir, float moveSpeed)
    {
        idleTimer = 0f;
        if (anim != null) anim.SetBool("moving", true);

        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);

        enemy.position = new Vector3(
            enemy.position.x + moveSpeed * dir * Time.deltaTime,
            enemy.position.y,
            enemy.position.z
        );
    }

    private void Chase()
    {
        if (player == null) return;

        if (anim != null) anim.SetBool("moving", true);
        if (anim != null && anim.HasParameter("isChasing")) anim.SetBool("isChasing", true);

        float dx = player.position.x - enemy.position.x;
        float absDx = Mathf.Abs(dx);

        if (absDx <= stopDistance)
        {
            if (anim != null) anim.SetBool("moving", false);
            return;
        }

        if (lockWithinBounds)
        {
            float nextX = enemy.position.x + Mathf.Sign(dx) * chaseSpeed * Time.deltaTime;
            if (nextX < leftEdge.position.x || nextX > rightEdge.position.x)
            {
                enemy.localScale = new Vector3(Mathf.Sign(dx) * Mathf.Abs(initScale.x), initScale.y, initScale.z);
                return;
            }
        }

        enemy.localScale = new Vector3(Mathf.Sign(dx) * Mathf.Abs(initScale.x), initScale.y, initScale.z);

        enemy.position = new Vector3(
            enemy.position.x + Mathf.Sign(dx) * chaseSpeed * Time.deltaTime,
            enemy.position.y,
            enemy.position.z
        );
    }

    private bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector2.Distance(enemy.position, player.position) <= detectionRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((enemy ? enemy.position : transform.position), detectionRadius);
    }
}

static class AnimatorExtensions
{
    public static bool HasParameter(this Animator animator, string paramName)
    {
        if (animator == null) return false;
        foreach (var p in animator.parameters)
            if (p.name == paramName) return true;
        return false;
    }
}
