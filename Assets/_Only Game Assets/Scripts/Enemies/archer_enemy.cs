using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class archer_enemy : MonoBehaviour
{
    public GameObject player;
    [SerializeField] GameObject arrow;
    BoxCollider2D col;
    bool shoot_arrow = true;
    Rigidbody2D rb;

    Quaternion rotation;
    Vector2 movedir;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthUI healthUI;

    [SerializeField] float range_of_player;
    [SerializeField] float speed;
    [SerializeField] float arrow_speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        if (healthUI != null)
        {
            healthUI.SetMaxHealth(maxHealth);
            healthUI.SetHealth(currentHealth); // to initialize UI
        }
        col = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
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

        if (transform.rotation == rotation && shoot_arrow)
        {
            if (Vector2.Distance(transform.position, player.transform.position) > range_of_player)
                StartCoroutine(fire(4));
            else
                StartCoroutine(fire(8));
                
        }
    }

    IEnumerator fire(float time)
    {

        GameObject bullet = Instantiate(arrow, gameObject.transform.position, gameObject.transform.rotation);
        shoot_arrow = false;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = transform.up * arrow_speed;
        yield return new WaitForSeconds(time);
        shoot_arrow = true;
        Destroy(bullet);
    }
}
