using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collided");
        var healthSystem = collision.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damageAmount);
        }
    }
}

