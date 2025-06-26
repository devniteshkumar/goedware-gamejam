using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;

public class enemy_healer : MonoBehaviour
{
    public static List<GameObject> enemiesToHeal = new List<GameObject>(); // Shared globally

    GameObject current_enemy = null;
    quaternion rotation;
    Vector2 movedir;

    void Update()
    {
        if (current_enemy == null)
        {
            current_enemy = FindNextEnemyToHeal();
        }
        else
        {
            HealthSystem hs = current_enemy.GetComponent<HealthSystem>();
            if (hs.currentHealth < hs.maxHealth && !hs.dead)
            {
                RotateEnemy(current_enemy);
                hs.Heal(10 * Time.deltaTime);
            }
            else
            {
                current_enemy = null;
            }
        }
    }

    GameObject FindNextEnemyToHeal()
    {
        foreach (GameObject enemy in enemiesToHeal)
        {
            if (enemy == null) continue;

            HealthSystem hs = enemy.GetComponent<HealthSystem>();
            if (hs != null && hs.currentHealth < hs.maxHealth && !hs.dead)
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200 * Time.deltaTime);
    }
}