using UnityEngine;

public class HealthSystem : MonoBehaviour
{
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
        GameManager.Instance.debugMessageTextToShow = "You Died!";
    }
}

