using UnityEngine;

public class Minion : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 0.5f;
    public float attackRange = 1.5f;
    public float damageInterval = 1f; // DPS interval

    private Transform target;
    private bool isGood = false;
    private float damageTimer = 0f;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        speed += Random.Range(-1f, 1f);
    }

    void Update()
    {
        if (!target) return;

        Vector2 directionToTarget = (target.position - transform.position);
        float distance = directionToTarget.magnitude;
        directionToTarget.Normalize();

        if (distance > attackRange)
        {
            transform.position += (Vector3)(directionToTarget * speed * Time.deltaTime);
        }

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
        else if (isGood && !other.CompareTag("Enemy"))
        {
            BecomeGood();
        }
    }

    public void BecomeGood()
    {
        isGood = true;
        gameObject.tag = "GoodMinion";

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

        target = closestEnemy != null ? closestEnemy : GameObject.FindWithTag("Player").transform;
    }
}
