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
        //remvoing movement for archer

        // if (Vector2.Distance(transform.position, player.transform.position) > range_of_player)
        // {
        //     MoveEnemy();

        // }

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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200 * Time.deltaTime);
        animator.SetFloat("move_x", movedir.x);
        animator.SetFloat("move_y", movedir.y);
        if (transform.rotation == rotation && shoot_arrow)
        {
            if (Vector2.Distance(transform.position, player.transform.position) > range_of_player)
                StartCoroutine(fire(3));
            else
                StartCoroutine(fire(6));

        }
    }

    IEnumerator fire(float time)
    {


        shoot_arrow = false;
        animator.SetBool("shoot", true);


        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animDuration = stateInfo.length;

        yield return new WaitForSeconds(animDuration + 0.2f);
        GameObject bullet = Instantiate(arrow, gameObject.transform.position, gameObject.transform.rotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = transform.up * arrow_speed;
        bullet.transform.Rotate(new Vector3(0, 0, 90));
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
