using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Points de patrouille")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Référence Ennemi")]
    [SerializeField] private Transform enemy;   

    [Header("Mouvement")]
    [SerializeField] private float speed = 2f;
    private Vector3 initScale;
    private bool movingLeft;

    [Header("Idle")]
    [SerializeField] private float idleDuration = 1f;
    private float idleTimer;

    [Header("Animator")]
    [SerializeField] private Animator anim;     

    private void Awake()
    {
        if (enemy == null) enemy = transform;               
        initScale = enemy.localScale;

        if (anim == null)                                  
            anim = enemy.GetComponent<Animator>();
        if (anim == null)                                  
            anim = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        if (anim != null) anim.SetBool("moving", false);
    }

    private void Update()
    {
        if (leftEdge == null || rightEdge == null) return;

        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
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

    private void MoveInDirection(int dir)
    {
        idleTimer = 0f;
        if (anim != null) anim.SetBool("moving", true);

        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);

        enemy.position = new Vector3(
            enemy.position.x + speed * dir * Time.deltaTime,
            enemy.position.y,
            enemy.position.z
        );
    }
}
