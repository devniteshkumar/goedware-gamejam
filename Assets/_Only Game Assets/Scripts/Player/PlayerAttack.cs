using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackDamage;

    private void Update()
    {
        attackDamage = SpecialAbilityManager.GetResource(ResourceTypes.AttackDamage).amount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<HealthSystem>().TakeDamage(attackDamage);
        }
    }
}
