using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour
{
    public enum ActorType { Player, Enemy }

    [Header("Who am I?")]
    [SerializeField] private ActorType actorType = ActorType.Player;

    [Header("Health")]
    [SerializeField] private float startingHealth = 3f;
    public float currentHealth { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioClip Dead;
    [SerializeField] private AudioClip Hurt;

    [Header("Death / Anim")]
    [SerializeField] private string deathStateName = "Die";    
    [SerializeField] private float extraDestroyDelay = 0.1f;  
    [SerializeField] private float playerRespawnDelay = 2f;

    private Animator anim;
    private bool dead;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private PlayerMovement movement;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TakeDamage(float _damage)
    {
        if (dead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            if (anim) anim.SetTrigger("hurt");
            if (audioSource && Hurt) audioSource.PlayOneShot(Hurt);
        }
        else
        {
            if (!dead)
            {
                dead = true;

                if (rb) rb.linearVelocity = Vector2.zero;
                if (movement) movement.enabled = false;

                if (anim) anim.SetTrigger("die");
                if (audioSource && Dead) audioSource.PlayOneShot(Dead);

                if (actorType == ActorType.Player)
                {
                    StartCoroutine(RespawnAfterDelay(playerRespawnDelay));
                }
                else 
                {
                    StartCoroutine(HandleEnemyDeath());
                }
            }
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    private IEnumerator RespawnAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        RespawnAtCheckpoint();
    }

    private void RespawnAtCheckpoint()
    {
        if (CheckpointManager.Instance == null)
        {
            RestartLevel();
            return;
        }

        transform.position = CheckpointManager.Instance.SpawnPoint;

        currentHealth = startingHealth;
        dead = false;

        if (rb) rb.linearVelocity = Vector2.zero;
        if (movement) movement.enabled = true;

        if (anim)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }

    private IEnumerator HandleEnemyDeath()
    {
        DisableAllColliders();
        DisableCommonMovementScripts();

        float wait = 0.35f;
        if (anim)
        {
            yield return null;

            yield return new WaitUntil(() =>
            {
                var st = anim.GetCurrentAnimatorStateInfo(0);
                return st.IsName(deathStateName) || st.tagHash == Animator.StringToHash("Death");
            });

            var state = anim.GetCurrentAnimatorStateInfo(0);
            wait = state.length;
        }

        yield return new WaitForSeconds(wait + extraDestroyDelay);

        Destroy(gameObject);
    }

    private void DisableAllColliders()
    {
        var cols = GetComponentsInChildren<Collider2D>(includeInactive: false);
        foreach (var c in cols) c.enabled = false;
    }

    private void DisableCommonMovementScripts()
    {
        var mover = GetComponent<MonoBehaviour>();

    }
}
