using System.Collections.Generic;
using UnityEngine;

public class RangedWithMeleeEnemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Enemy Properties")]
    public float moveSpeed = 3f;
    public float minDistanceWithPlayer = 10f;

    [Header("Minion Properties")]
    public GameObject minionPrefab;
    public float spawnCooldown = 3f;
    public float spawnRadius = 2f;
    
    [Header("Missile Properties")]
    public GameObject missilePrefab;
    public float missileCooldown = 10f;
    public float maxSize = 5;
    public float minSize = 1;
    public float sizeIncrementationRate = 0.5f;

    [Header("Properties")]
    private float missileTimer;
    private float spawnTimer;
    private bool isRunning;
    private List<Missile> missiles = new();

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        missileTimer = missileCooldown;
        spawnTimer = spawnCooldown;
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 dir = (transform.position - player.position).normalized;

        if (distance < minDistanceWithPlayer)
        {
            isRunning = true;
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)dir, moveSpeed * Time.deltaTime);  //Champt GPT
        }
        else
        {
            isRunning = false;
        }

        HandleMissileAttack(dir);
        HandleMinionSpawn();
        HandleMissileMovementAndDamage();
    }

    void HandleMissileAttack(Vector2 dir)
    {
        missileTimer -= Time.deltaTime;
        if (missileTimer <= 0f && !isRunning)
        {
            FireMissile(dir);
            missileTimer = missileCooldown;
        }
    }

    void FireMissile(Vector2 direction)
    {
        GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity);
        missiles.Add(new Missile(missile, (player.transform.position - transform.position)/2));
        missile.GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * 5f;
    }

    private void HandleMissileMovementAndDamage()
    {
        foreach (var missile in missiles)
        {
            float dot = Vector2.Dot(missile.rb.linearVelocity, missile.center - (Vector2)missile.missile.transform.position);
            int result = dot < 0 ? -1 : (dot > 0 ? 1 : 0);

            missile.missile.transform.localScale += Vector3.one * sizeIncrementationRate * Time.deltaTime * result;
            
            if ((missile.missile.transform.position - player.transform.position).magnitude > missile.travelDistance)
            {
                //TODO Explode
                missiles.Remove(missile);
                Destroy(missile.missile);
            }
        }
    }

    void HandleMinionSpawn()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && !isRunning)
        {
            SpawnMinion();
            spawnTimer = spawnCooldown;
        }
    }

    void SpawnMinion()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 pos = transform.position + (Vector3)dir * spawnRadius;
        Instantiate(minionPrefab, pos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }

    public void OnDeath()
    {
        //make them green and go player side
    }
}

public class Minion : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 10;
    public int health = 1;
    private Transform player;
    private bool isFriendly = false;

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

    public void BecomeFriendly()
    {
        isFriendly = true;
        // Optional: change tag, color, behavior, etc.
        gameObject.tag = "PlayerAlly";
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFriendly && other.CompareTag("Player"))
        {
            // Damage the player here
            Destroy(gameObject);
        }
    }
}

public class Missile
{
    public GameObject missile;
    public Vector2 center;
    public Rigidbody2D rb;
    public float travelDistance;

    public Missile(GameObject missile, Vector2 center)
    {
        this.missile = missile;
        this.center = center;
        rb = missile.GetComponent<Rigidbody2D>();
        this.travelDistance = ((Vector2)missile.transform.position - center).magnitude * 2;
    }
}