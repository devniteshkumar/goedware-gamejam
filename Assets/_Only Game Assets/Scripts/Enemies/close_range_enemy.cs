using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class close_range_enemy : MonoBehaviour
{
    quaternion rotation;

    public Transform attack_point;
    public float attack_raidus;

    public float attack_distance_from_center;
    public LayerMask player_side_mask;
    public Animator animator;
    public GameObject player;

    private Rigidbody2D rb;
    private Vector2 movedir;

    bool isAttacking = true;

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


        if (HealthSystem.currentHealth < 2 && !HealthSystem.dead)
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
        else if(isAttacking)
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

        if (movedir.x > 0.7f)
            attack_point.localPosition = new Vector3(attack_distance_from_center, 0, 0);
        else if (movedir.x < -0.7f)
            attack_point.localPosition = new Vector3(-attack_distance_from_center, 0, 0);
        else if (movedir.y < -0.7f)
            attack_point.localPosition = new Vector3(0, -attack_distance_from_center, 0);
        else
            attack_point.localPosition = new Vector3(0, attack_distance_from_center, 0);


        // Update animator with move direction
        animator.SetFloat("move_x", movedir.x);
        animator.SetFloat("move_y", movedir.y);
    }
    // void RotateEnemy()
    // {
    //     movedir = (player.transform.position - transform.position).normalized;
    //     rotation = Quaternion.LookRotation(Vector3.forward, movedir);
    //     transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200 * Time.deltaTime);
    // }

    IEnumerator attack()
    {
        isAttacking = false;
        animator.SetBool("attack", true);
        yield return null;

        Collider2D[] player_side = Physics2D.OverlapCircleAll(attack_point.position, attack_raidus, player_side_mask);
        foreach (Collider2D player_side_single in player_side)
        {
            player_side_single.GetComponent<HealthSystem>().TakeDamage(10);
            Debug.Log(player_side_single.name);
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animDuration = stateInfo.length;

        yield return new WaitForSeconds(animDuration + 0.3f);
        animator.SetBool("attack", false);
        isAttacking = true;

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





