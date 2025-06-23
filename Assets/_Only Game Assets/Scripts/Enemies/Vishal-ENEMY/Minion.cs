using UnityEngine;


public class Minion : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 10;
    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (!player) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Damage the player here
        }
    }
}