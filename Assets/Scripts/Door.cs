using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OuvrePorteFinJeu : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("GameObject de la porte (avec un Animator)")]
    public GameObject porte;

    [Tooltip("Point d'aspiration du joueur (par défaut: centre de la porte)")]
    public Transform pointAspiration;

    [Header("Timing")]
    [Tooltip("Attente avant de commencer l'aspiration (laisse l'anim d'ouverture démarrer)")]
    public float delaiOuverture = 0.0f;

    [Tooltip("Durée de l'aspiration (plus grand = plus lent)")]
    public float dureeAspiration = 2f;

    [Tooltip("Petit délai après disparition du joueur avant de finir")]
    public float delaiFin = 0.3f;

    [Header("Fin du jeu")]
    [Tooltip("True = charger une scène; False = quitter l'application")]
    public bool chargerSceneSuivante = false;

    [Tooltip("Nom de la scène si 'chargerSceneSuivante' est coché")]
    public string nomSceneSuivante = "Credits";

    private Animator animPorte;
    private bool aDejaDeclenche = false;

    private void Start()
    {
        if (porte != null)
            animPorte = porte.GetComponent<Animator>();

        if (pointAspiration == null && porte != null)
            pointAspiration = porte.transform;
    }

    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (aDejaDeclenche) return;
        if (!autre.CompareTag("Player")) return;

        aDejaDeclenche = true;

        if (animPorte != null)
            animPorte.SetTrigger("OpenDoor");

        StartCoroutine(AspirerPuisTerminer(autre.gameObject));
    }

    private IEnumerator AspirerPuisTerminer(GameObject joueur)
    {
        

        var rb = joueur.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;    
            rb.angularVelocity = 0f;
            rb.isKinematic = true;         
            rb.simulated = true;           
        }

        if (delaiOuverture > 0f)
        {
            float t0 = 0f;
            while (t0 < delaiOuverture)
            {
                t0 += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        var rendus = joueur.GetComponentsInChildren<SpriteRenderer>(true);

        Vector3 posDepart = joueur.transform.position;
        Vector3 posCible = pointAspiration != null ? pointAspiration.position : posDepart;
        Vector3 echelleDepart = joueur.transform.localScale;

        Color[] couleursDepart = new Color[rendus.Length];
        for (int i = 0; i < rendus.Length; i++)
            if (rendus[i] != null) couleursDepart[i] = rendus[i].color;

        float t = 0f;
        while (t < dureeAspiration)
        {
            float u = t / dureeAspiration;

            float k = 1f - Mathf.Pow(1f - u, 3f);

            joueur.transform.position = Vector3.Lerp(posDepart, posCible, k);

            joueur.transform.localScale = Vector3.Lerp(echelleDepart, Vector3.zero, k);

            for (int i = 0; i < rendus.Length; i++)
            {
                var sr = rendus[i];
                if (sr == null) continue;
                Color c = couleursDepart[i];
                c.a = Mathf.Lerp(c.a, 0f, k);
                sr.color = c;
            }

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        for (int i = 0; i < rendus.Length; i++)
        {
            if (rendus[i] == null) continue;
            var c = rendus[i].color;
            c.a = 0f;
            rendus[i].color = c;
        }
        joueur.transform.localScale = Vector3.zero;

        if (delaiFin > 0f)
        {
            float e = 0f;
            while (e < delaiFin)
            {
                e += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        if (chargerSceneSuivante && !string.IsNullOrEmpty(nomSceneSuivante))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nomSceneSuivante);
        }
        else
        {
            UnityEditor.EditorApplication.isPlaying = false; 
            Application.Quit();

        }
    }

    private void OnTriggerExit2D(Collider2D autre)
    {
    }
}
