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
    public Transform attack_point;
    public float attack_distance_from_center;

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


        if (shoot_arrow && transform.rotation == rotation)
        {
            StartCoroutine(fire(3));
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
        
        // if (movedir.x > 0.7f)
        //     attack_point.localPosition = new Vector3(attack_distance_from_center, 0, 0);
        // else if (movedir.x < -0.7f)
        //     attack_point.localPosition = new Vector3(-attack_distance_from_center, 0, 0);
        // else if (movedir.y < -0.7f)
        //     attack_point.localPosition = new Vector3(0, -attack_distance_from_center, 0);
        // else
        //     attack_point.localPosition = new Vector3(0, attack_distance_from_center, 0);
    }

    [System.Obsolete]
    IEnumerator fire(float time)
    {
        shoot_arrow = false;

        animator.SetBool("shoot", true);

        // ✅ Give Animator time to enter "arrow_down"
        yield return new WaitForSeconds(0.05f);

        // ✅ Safely get animation length
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animDuration = stateInfo.length > 0 ? stateInfo.length : 0.5f;

        yield return new WaitForSeconds(animDuration * 0.6f); // fire mid-animation

        GameObject bullet = Instantiate(arrow, attack_point.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = transform.up * arrow_speed;
        bullet.transform.Rotate(0, 0, 90);

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
