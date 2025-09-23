using UnityEngine;
using UnityEngine.SceneManagement;


public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] AudioClip Dead;
    [SerializeField] AudioClip Hurt;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;
    private AudioSource audioSource;


    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            audioSource.PlayOneShot(Hurt);

        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");
                audioSource.PlayOneShot(Dead);

                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
                Invoke("RestartLevel", 2f);
            }
        }
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }


}