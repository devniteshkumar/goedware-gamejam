using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;

public class enemy_healer : MonoBehaviour
{
    public flash flash;

    [SerializeField]float heal_radius;
    [SerializeField]float healer_speed;
    [SerializeField]float heal_per_sec;
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
            StartCoroutine(PlayDeathAnimation(transform)); 
        }



        if (current_enemy == null)
        {
            current_enemy = FindNextEnemyToHeal();
        }
        else
        {
            HealthSystem healthSystem = current_enemy.GetComponent<HealthSystem>();
            movedir = current_enemy.transform.position - transform.position;
            movedir.Normalize();
            if (healthSystem.currentHealth < healthSystem.maxHealth && !healthSystem.dead)
            {

                if (Vector3.Distance(current_enemy.transform.position, transform.position) > heal_radius)
                {
                    transform.position = Vector2.MoveTowards(transform.position, current_enemy.transform.position, healer_speed * Time.deltaTime);
                    animator.SetFloat("move_x", movedir.x);
                    animator.SetFloat("move_y", movedir.y);
                }
                else
                {
                    animator.SetBool("heal", true);
                    healthSystem.Heal(heal_per_sec * Time.deltaTime);
                }

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