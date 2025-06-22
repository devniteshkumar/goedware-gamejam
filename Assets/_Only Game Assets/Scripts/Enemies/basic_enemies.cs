using Unity.VisualScripting;
using UnityEngine;

public class basic_enemies : MonoBehaviour
{
    public GameObject player;
    BoxCollider2D col;

    Rigidbody2D rb;

    Quaternion rotation;
    Vector2 movedir;



    [SerializeField] float range_of_player;
    [SerializeField] float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //either we can make it stop before hitting player and do a sword attack
        if (Vector2.Distance(transform.position, player.transform.position) > range_of_player)
        {
            MoveEnemy();
            RotateEnemy();
        }
        //else do it always and make it player
        // MoveEnemy();
        // RotatEenemy();
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
        

    }

}
