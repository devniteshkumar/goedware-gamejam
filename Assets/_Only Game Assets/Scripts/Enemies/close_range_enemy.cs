using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class close_range_enemy : MonoBehaviour
{
    quaternion rotation;
    public Animator animator;
    public GameObject player;

    private Rigidbody2D rb;
    private Vector2 movedir;

    public HealthUI healthUI;
    public HealthSystem HealthSystem;
    private enemy_healer enemy_healer;

    [SerializeField] float range_of_player = 1.5f;
    [SerializeField] float speed = 2f;

    [System.Obsolete]

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemy_healer = FindObjectOfType<enemy_healer>();
        
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HealthSystem.TakeDamage(20);

        }


        if (HealthSystem.currentHealth <2 && !HealthSystem.dead)
        {
            HealthSystem.dead = true;
            StartCoroutine(death());
        }

        if (HealthSystem.currentHealth != HealthSystem.maxHealth)
        {
            enemy_healer.enemiesToHeal.Add(gameObject);
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > range_of_player && !HealthSystem.dead)
        {
            MoveEnemy();
        }
        else
        {
            StartCoroutine(attack());
        }
    }

    void MoveEnemy()
    {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = player.transform.position;
        movedir = (targetPosition - currentPosition).normalized;

        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        // Update animator with move direction
        animator.SetFloat("move_x", movedir.x);
        animator.SetFloat("move_y", movedir.y);
    }
    void RotateEnemy()
    {
        movedir = (player.transform.position - transform.position).normalized;
        rotation = Quaternion.LookRotation(Vector3.forward, movedir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200 * Time.deltaTime);
    }

    IEnumerator attack()
    {

        animator.SetBool("attack", true);
        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animDuration = stateInfo.length;

        yield return new WaitForSeconds(animDuration + 0.3f);
        animator.SetBool("attack", false);

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
}





