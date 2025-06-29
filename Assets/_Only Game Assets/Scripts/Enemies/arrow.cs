using UnityEngine;

public class arrow : MonoBehaviour
{
    private BoxCollider2D col;
    public LayerMask player_side_mask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & player_side_mask) != 0 || collision.gameObject.CompareTag("GoodMinion"))
        {
            collision.gameObject.GetComponent<HealthSystem>().TakeDamage(10);
            Destroy(gameObject);
        }
        else if (((1 << collision.gameObject.layer) & player_side_mask) == 0 && !collision.gameObject.CompareTag("archer"))
        {
            Destroy(gameObject);
        }
    }
}
