using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private AudioClip sfxJump;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private AudioSource audioSource;

    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float h = 0f;
        if (Input.GetKey(KeyCode.RightArrow)) h += 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) h -= 1f;
        horizontalInput = Mathf.Clamp(h, -1f, 1f);

        if (horizontalInput > 0.01f) transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f) transform.localScale = new Vector3(-1, 1, 1);

        anim.SetBool("Run", Mathf.Abs(horizontalInput) > 0.01f);
        anim.SetBool("Grounded", IsGrounded());

        if (wallJumpCooldown > 0.2f)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (OnWall() && !IsGrounded())
            {
                body.gravityScale = 0f;
                body.linearVelocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 3f;
            }

            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("Jump");
            if (audioSource && sfxJump) audioSource.PlayOneShot(sfxJump);
        }
        else if (OnWall() && !IsGrounded())
        {
            if (Mathf.Abs(horizontalInput) < 0.01f)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10f, 0f);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3f, 6f);
            }
            wallJumpCooldown = 0f;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private bool OnWall()
    {
        Vector2 dir = new Vector2(transform.localScale.x, 0f);
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, dir, 0.1f, wallLayer);
        return hit.collider != null;
    }

    public bool canAttack()
    {
        return IsGrounded() && !OnWall();
    }
}
