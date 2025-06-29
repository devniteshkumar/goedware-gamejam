using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public bool dead = false;
    public float maxHealth = 100;
    public float currentHealth;
    public HealthUI healthUI;
    void Start()
    {
        currentHealth = maxHealth;
        if (healthUI != null)
        {
            healthUI.SetMaxHealth(maxHealth);
            healthUI.SetHealth(currentHealth); // to initialize UI
        }
    }

    private void Update()
    {
        if (healthUI.health != currentHealth)
        {
            healthUI.SetHealth(currentHealth);
        }
        if (healthUI.maxHealth != maxHealth)
        {
            healthUI.SetMaxHealth(maxHealth);
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0 && (gameObject.tag == "Enemy" || gameObject.tag == "Minion" || gameObject.tag == "GoodMinion"))
        {
            SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject, 2f);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0); // prevent negative values

        if (healthUI != null)
            healthUI.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        SendMessage("OnDamaged", SendMessageOptions.DontRequireReceiver);
        if (gameObject.tag == "Player")
        {
            audio_manager.Instance.PlaySound(audio_manager.Instance.player_hit);
        }
        else
        {
            audio_manager.Instance.PlaySound(audio_manager.Instance.enemy_hit);
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthUI != null)
            healthUI.SetHealth(currentHealth);
    }
    IEnumerator PlayDeathAnimation(Transform obj)
    {
        float duration = 1f;
        float elapsed = 0f;
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Color startColor = sr.color;
        Vector3 originalScale = obj.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Fade out
            sr.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);

            // Shrink
            obj.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

            // Rotate
            obj.Rotate(0, 0, 360f * Time.deltaTime); // rotate 360 degrees per second

            yield return null;
        }

        Destroy(obj.gameObject);
    }
    void Die()
    {
        // Handle death logic here
        if (gameObject.CompareTag("Player"))
        {
            GameManager.Instance.debugMessageTextToShow = "You Died!";
            StartCoroutine(PlayDeathAnimation(transform));
            audio_manager.Instance.PlaySound(audio_manager.Instance.death);
        }
        SceneController.instance.Lose();
    }
}

