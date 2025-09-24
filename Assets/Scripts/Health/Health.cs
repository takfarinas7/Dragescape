using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3;
    [SerializeField] private AudioClip Dead;
    [SerializeField] private AudioClip Hurt;

    public float currentHealth { get; private set; }

    private Animator anim;
    private bool dead;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private PlayerMovement movement; // ton script mouvement

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
    }

    // Gard� au cas o� tu veux vraiment reload la sc�ne ailleurs
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
                if (anim) anim.SetTrigger("die");
                if (audioSource && Dead) audioSource.PlayOneShot(Dead);
                if (movement) movement.enabled = false;
                if (rb) rb.linearVelocity = Vector2.zero;

                // Respawn apr�s ~2s
                StartCoroutine(RespawnAfterDelay(2f));
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
        // Si pas de manager, fallback = reload sc�ne
        if (CheckpointManager.Instance == null)
        {
            RestartLevel();
            return;
        }

        // T�l�portation au dernier checkpoint
        transform.position = CheckpointManager.Instance.SpawnPoint;

        // Reset �tat
        currentHealth = startingHealth;
        dead = false;

        if (rb) rb.linearVelocity = Vector2.zero;
        if (movement) movement.enabled = true;

        // Remet l'anim proprement sans conna�tre le nom de l'�tat Idle
        if (anim)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }
}
