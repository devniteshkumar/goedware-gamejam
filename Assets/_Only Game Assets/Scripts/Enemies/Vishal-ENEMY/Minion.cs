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
    private Animator animator;

    private Vector3 lastPosition;
    private Vector3 lastVelocity;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }


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

        // Animate
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        PassAnimVars(velocity);
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
                animator.SetTrigger("attack");
            }
        }
        else if (isGood && other.CompareTag("Enemy"))
        {
            var healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
                damageTimer = damageInterval;
                animator.SetTrigger("attack"); 
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
        gameObject.layer = LayerMask.NameToLayer("Enemy");

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

    private void PassAnimVars(Vector3 velocity)
    {
        bool isMoving = velocity.sqrMagnitude > 0.001f;

        if (isMoving)
            lastVelocity = velocity; // Only update when there's real movement

        Vector3 animVelocity = isMoving ? velocity : lastVelocity;

        animator.SetBool("moving", isMoving);
        animator.SetFloat("AnimMoveX", animVelocity.x);
        animator.SetFloat("AnimMoveY", animVelocity.y);
    }

    public void OnDamaged()
    {
        animator.SetTrigger("hurt");
    }
    
    public void OnDeath()
    {
        target = null;
        animator.SetTrigger("dead");
    }
}



