using System.Collections;
using UnityEngine;
using Unity.Mathematics;

public class close_range_enemy : MonoBehaviour
{
    public flash flash;

    public Transform attack_point;
    public float attack_raidus;
    public float attack_distance_from_center;
    public LayerMask player_side_mask;
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 movedir;

    public GameObject target;

    bool isAttacking = true;
    private Coroutine attackCoroutine;

    float attackCooldown = 0f; // tracks time left before next attack
    float attackRate = 1.2f; // seconds between attacks
    public HealthUI healthUI;
    public HealthSystem HealthSystem;
    private enemy_healer enemy_healer;

    [SerializeField] float range_of_player = 1.5f;
    [SerializeField] float speed = 2f;

    [System.Obsolete]
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_healer = FindObjectOfType<enemy_healer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.SetTrigger("Attack");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("=== TESTING TARGETS ===");

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject[] minions = GameObject.FindGameObjectsWithTag("goodminion");

            Debug.Log("Player found: " + (player != null));
            Debug.Log("Minions found: " + minions.Length);

            foreach (GameObject m in minions)
            {
                Debug.Log("Minion: " + m.name);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HealthSystem.TakeDamage(20);
            flash.Flash();
        }

        if (HealthSystem.currentHealth < 2 && !HealthSystem.dead)
        {
            HealthSystem.dead = true;
            StartCoroutine(death());
        }

        if (HealthSystem.currentHealth != HealthSystem.maxHealth)
        {
            if (!enemy_healer.enemiesToHeal.Contains(gameObject))
            {
                enemy_healer.enemiesToHeal.Add(gameObject);
            }
        }

        if (HealthSystem.dead) return;

        FindNearestTarget();
        if (target == null) return;

    float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

    if (distanceToTarget > range_of_player)
    {
        MoveEnemy();
    }
    else
    {

        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f)
        {
            AttackTarget();
            attackCooldown = attackRate;
        }
    }
    }

    void FindNearestTarget()
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("GoodMinion");
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        float closestDist = Mathf.Infinity;
        GameObject closest = null;

        if (player != null)
        {
            float playerDist = Vector2.Distance(transform.position, player.transform.position);
            if (playerDist < closestDist)
            {
                closest = player;
                closestDist = playerDist;
            }
        }

        foreach (GameObject minion in allTargets)
        {
            if (minion == null) continue;
            float minionDist = Vector2.Distance(transform.position, minion.transform.position);
            if (minionDist < closestDist)
            {
                closest = minion;
                closestDist = minionDist;
            }
        }

        if (closest != target)
        {
            Debug.Log("Target changed to: " + closest?.name);
            target = closest;
            // Optionally: reset coroutine
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
                isAttacking = true;
            }
        }
    }

    void MoveEnemy()
    {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = target.transform.position;
        movedir = (targetPosition - currentPosition).normalized;

        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        UpdateAttackPoint();
        animator.SetFloat("move_x", movedir.x);
        animator.SetFloat("move_y", movedir.y);
    }


    void UpdateAttackPoint()
    {
        if (movedir.x > 0.7f)
            attack_point.localPosition = new Vector3(attack_distance_from_center, 0, 0);
        else if (movedir.x < -0.7f)
            attack_point.localPosition = new Vector3(-attack_distance_from_center, 0, 0);
        else if (movedir.y < -0.7f)
            attack_point.localPosition = new Vector3(0, -attack_distance_from_center, 0);
        else
            attack_point.localPosition = new Vector3(0, attack_distance_from_center, 0);
    }


    void AttackTarget()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance > range_of_player) return;

        animator.SetTrigger("Attack");

        StartCoroutine(DealDamageAfterDelay(0.3f));
    }

    IEnumerator DealDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attack_point.position, attack_raidus, player_side_mask);
        foreach (Collider2D col in hitTargets)
        {
            HealthSystem h = col.GetComponent<HealthSystem>();
            if (h != null)
            {
                h.TakeDamage(10);
                Debug.Log("Attacked: " + col.name);
            }
        }
    }

    IEnumerator death()
    {
        animator.SetBool("dead", true);
        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animDuration = stateInfo.length;

        yield return new WaitForSeconds(animDuration + 0.1f);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = attack_point == null ? Vector3.zero : attack_point.position;
        Gizmos.DrawWireSphere(position, attack_raidus);
    }
}
