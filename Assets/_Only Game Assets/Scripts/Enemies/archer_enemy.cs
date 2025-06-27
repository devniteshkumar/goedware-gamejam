using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public HealthSystem HealthSystem;
    public HealthUI healthUI;
    enemy_healer enemy_healer;
    [SerializeField] float range_of_player;
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
        if (HealthSystem.currentHealth != HealthSystem.maxHealth)
        {
            enemy_healer.enemiesToHeal.Add(gameObject);
        }

        if (HealthSystem.currentHealth < 2 && !HealthSystem.dead)
        {
            HealthSystem.dead = true;
            StartCoroutine(death());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            var healthSystem = gameObject.GetComponent<HealthSystem>();
            healthSystem.TakeDamage(20);

        }

        RotateEnemy();


        if (shoot_arrow)
        {
            if (Vector2.Distance(transform.position, player.transform.position) > range_of_player)
                StartCoroutine(fire(3));
            else
                StartCoroutine(fire(6));

        }

    }
    void MoveEnemy()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    void RotateEnemy()
    {
        movedir = player.transform.position - transform.position;
        movedir.Normalize();
        rotation = Quaternion.LookRotation(Vector3.forward, movedir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 100 * Time.deltaTime);
        animator.SetFloat("move_x", movedir.x);
        animator.SetFloat("move_y", movedir.y);
    }

    [System.Obsolete]
        IEnumerator fire(float time)
        {
            shoot_arrow = false; // <-- move this up immediately to prevent multiple fires

            animator.SetBool("shoot", true);
            yield return null; // allow Animator to update state

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animDuration = stateInfo.length;

            yield return new WaitForSeconds(animDuration);

            GameObject bullet = Instantiate(arrow, transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = transform.up * arrow_speed;
            bullet.transform.Rotate(0, 0, 90);

            animator.SetBool("shoot", false);

            yield return new WaitForSeconds(time);
            shoot_arrow = true;

            Destroy(bullet); // optional: maybe move this after a longer time if needed
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
