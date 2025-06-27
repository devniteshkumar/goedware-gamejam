using UnityEngine;

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

        if (currentHealth <= 0 && (gameObject.tag == "Enemy" || gameObject.tag == "Minion"))
        {
            SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject, 1.4f);
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

    void Die()
    {
        // Handle death logic here
        if (gameObject.CompareTag("Player"))
            GameManager.Instance.debugMessageTextToShow = "You Died!";
    }
}

