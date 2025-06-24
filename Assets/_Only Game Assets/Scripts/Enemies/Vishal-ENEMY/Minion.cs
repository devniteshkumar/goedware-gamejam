using UnityEngine;

public class Minion : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 0.5f;
    public float rotationSpeed = 360f;
    public float attackRange = 1.5f;
    public float damageInterval = 1f; // DPS interval

    private Transform target;
    private bool isGood = false;
    private float damageTimer = 0f;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        speed += Random.Range(-1f, 1f);
        rotationSpeed += Random.Range(-100, 100);
    }

    void Update()
    {
        if (!target) return;

        Vector2 directionToTarget = (target.position - transform.position);
        float distance = directionToTarget.magnitude;
        directionToTarget.Normalize();

        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (distance > attackRange)
        {
            Vector3 forward = transform.right;
            transform.position += forward * speed * Time.deltaTime;
        }

        // Reduce timer over time
        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (damageTimer > 0f) return;

        if (!isGood && other.CompareTag("Player"))
        {
            var healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
                damageTimer = damageInterval;
            }
        }
        else if (isGood && other.CompareTag("Enemy"))
        {
            var healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
                damageTimer = damageInterval;
            }
        }
    }

    public void BecomeGood()
    {
        isGood = true;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy;
        }
    }
}

