using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

public class archer_enemy : MonoBehaviour
{
    public Animator animator;
    public GameObject player;
    [SerializeField] GameObject arrow;
    BoxCollider2D col;
    bool shoot_arrow = true;
    Rigidbody2D rb;

    Quaternion rotation;
    Vector2 movedir;
    public Transform attack_point;
    public float attack_distance_from_center;

    public flash flash;
    public HealthSystem HealthSystem;
    public HealthUI healthUI;
    enemy_healer enemy_healer;
    [SerializeField] float range_of_player;
    [SerializeField] float min_distance_player;
    [SerializeField] float speed;
    [SerializeField] float arrow_speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [System.Obsolete]
    void Start()
    {

        col = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemy_healer = FindObjectOfType<enemy_healer>();
    }

    // Update is called once per frame
    [System.Obsolete]

    void Update()
    {
        if (HealthSystem.currentHealth != HealthSystem.maxHealth && !enemy_healer.enemiesToHeal.Contains(gameObject))
        {
            enemy_healer.enemiesToHeal.Add(gameObject);
        }

        if (HealthSystem.currentHealth < 2 && !HealthSystem.dead)
        {
            HealthSystem.dead = true;
            StartCoroutine(death());
            return;
        }



        if (player)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance < min_distance_player)
            {
                MoveAwayEnemy();
            }
            else if (distance > range_of_player)
            {
                MoveEnemy();
            }

            else
            {
                RotateEnemy();
                if (shoot_arrow)
                {
                    StartCoroutine(fire(3));
                }
            }
        }
    }
    void MoveAwayEnemy()
    {
        movedir = (transform.position - player.transform.position).normalized;

        animator.SetFloat("move_x", movedir.x);
        animator.SetFloat("move_y", movedir.y);

        transform.position += (Vector3)(movedir * speed * Time.deltaTime);

        UpdateAttackPoint();
    }


    void MoveEnemy()
    {
        movedir = player.transform.position - transform.position;
        movedir.Normalize();
        animator.SetFloat("move_x", movedir.x);
        animator.SetFloat("move_y", movedir.y);
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        UpdateAttackPoint();
    }

    void RotateEnemy()
    {
        movedir = player.transform.position - transform.position;
        movedir.Normalize();
        rotation = Quaternion.LookRotation(Vector3.forward, movedir);
        animator.SetFloat("move_x", movedir.x);
        animator.SetFloat("move_y", movedir.y);
        UpdateAttackPoint();
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

    IEnumerator fire(float time)
    {
        shoot_arrow = false;

        animator.SetBool("shoot", true);

        yield return new WaitForSeconds(0.05f);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animDuration = stateInfo.length > 0 ? stateInfo.length : 0.5f;

        yield return new WaitForSeconds(animDuration * 0.6f); // fire mid-animation

        // Calculate direction to player
        Vector2 direction = (player.transform.position - attack_point.position).normalized;

        GameObject bullet = Instantiate(arrow, attack_point.position, Quaternion.identity);

        if (bullet.TryGetComponent(out Rigidbody2D bulletRb))
        {
            bulletRb.linearVelocity = direction * arrow_speed;
        }

        // Optional: rotate the arrow to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        animator.SetBool("shoot", false);

        yield return new WaitForSeconds(time);
        shoot_arrow = true;

        Destroy(bullet);
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
