using UnityEngine;

public class playerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        bool firePressed = Input.GetButton("Fire1") || Input.GetKey(KeyCode.UpArrow);

        if (firePressed && cooldownTimer >= attackCooldown && playerMovement.canAttack())
        {
            Attack();
        }
    }

    private void Attack()
    {
        int idx = FindFireball();
        if (idx < 0)
            return;

        cooldownTimer = 0f;
        if (anim) anim.SetTrigger("Attack");

        GameObject fb = fireballs[idx];
        fb.transform.position = firePoint.position;

        float dir = Mathf.Sign(transform.localScale.x);
        fb.GetComponent<Projectile>().SetDirection(dir);
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (fireballs[i] != null && !fireballs[i].activeInHierarchy)
                return i;
        }
        return -1;
    }
}
