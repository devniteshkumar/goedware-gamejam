using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;

public class enemy_healer : MonoBehaviour
{
    public flash flash;
    public Animator animator;
    public static List<GameObject> enemiesToHeal = new List<GameObject>(); // Shared globally
    public HealthSystem HealthSystem;
    GameObject current_enemy = null;
    quaternion rotation;
    Vector2 movedir;

    void Update()
    {
        if (HealthSystem.currentHealth < 2 && !HealthSystem.dead)
        {
            HealthSystem.dead = true;
            StartCoroutine(PlayDeathAnimation(transform)); // 👈 Add this line
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            var healthSystem = gameObject.GetComponent<HealthSystem>();
            healthSystem.TakeDamage(20);
            flash.Flash();

        }


        if (current_enemy == null)
        {
            current_enemy = FindNextEnemyToHeal();
        }
        else
        {
            HealthSystem healthSystem = current_enemy.GetComponent<HealthSystem>();
            if (healthSystem.currentHealth < healthSystem.maxHealth && !healthSystem.dead)
            {
                animator.SetBool("heal", true);
                RotateEnemy(current_enemy);
                healthSystem.Heal(10 * Time.deltaTime);
            }
            else
            {
                animator.SetBool("heal", false);
                current_enemy = null;
            }
        }
    }

    GameObject FindNextEnemyToHeal()
    {
        foreach (GameObject enemy in enemiesToHeal)
        {
            if (enemy == null) continue;

            HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
            if (healthSystem != null && healthSystem.currentHealth < healthSystem.maxHealth && !healthSystem.dead)
            {
                return enemy;
            }
        }

        return null;
    }

    void RotateEnemy(GameObject enemy)
    {
        movedir = enemy.transform.position - transform.position;
        movedir.Normalize();
        rotation = Quaternion.LookRotation(Vector3.forward, movedir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 50 * Time.deltaTime);
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
}