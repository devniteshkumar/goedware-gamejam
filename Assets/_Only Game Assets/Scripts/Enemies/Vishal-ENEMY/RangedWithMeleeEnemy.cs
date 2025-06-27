using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWithMeleeEnemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform healer;
    public HealthSystem healthSystem;

    [Header("Enemy Properties")]
    public float moveSpeed = 3f;
    public float minDistanceWithPlayer = 10f;

    [Header("Minion Properties")]
    public GameObject minionPrefab;
    public float spawnCooldown = 3f;
    public float spawnRadius = 2f;
    public int maxMinionCount = 5;
    private List<GameObject> activeMinions = new();

    [Header("Missile Properties")]
    public GameObject missilePrefab;
    public GameObject damageArea;
    public float missileCooldown = 10f;
    public float missileSpeed = 5;
    public float maxSize = 5;
    public float minSize = 1;
    public float sizeIncrementationRate = 0.5f;

    [Header("Properties")]
    public float missileTimer;
    public float spawnTimer;
    public bool isRunning;
    public bool runTowardsHealer;
    public List<Missile> missiles = new();

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        healer = GameObject.FindWithTag("Healer")?.transform;
        missileTimer = missileCooldown;
        spawnTimer = spawnCooldown;
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnDisable()
    {
        OnDeath();
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 dir = (transform.position - player.position).normalized;
        Vector2 healDir = Vector2.zero;
        if (healer != null)
        {
            healDir = (healer.position - transform.position).normalized;
        }else
        {
            healer = GameObject.FindWithTag("Healer")?.transform;
        }

        if (distance < minDistanceWithPlayer)
        {
            isRunning = true;
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)dir, moveSpeed * Time.deltaTime);  //Champt GPT
        }
        else
        {
            isRunning = false;
        }
        if (healthSystem.currentHealth < healthSystem.maxHealth / 2 && healer != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)healDir, moveSpeed * Time.deltaTime);  //Champt GPT
            runTowardsHealer = true;
            isRunning = true;
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
        if (missile == null)
        {
            Debug.LogError("Missile instantiation failed!");
            return;
        }

        Vector2 midPoint = (player.position + transform.position) / 2;
        missiles.Add(new Missile(missile, midPoint));

        Rigidbody2D rb = missile.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Missile prefab missing Rigidbody2D!");
            return;
        }

        rb.linearVelocity = direction.normalized * (-missileSpeed);
    }


    private void HandleMissileMovementAndDamage()
    {
        for (int i = missiles.Count - 1; i >= 0; i--)
        {
            Missile missile = missiles[i];
            float dot = Vector2.Dot(missile.rb.linearVelocity, missile.center - (Vector2)missile.missile.transform.position);
            int result = dot < 0 ? -1 : (dot > 0 ? 1 : 0);

            missile.missile.transform.localScale += Vector3.one * sizeIncrementationRate * Time.deltaTime * result;

            if ((missile.missile.transform.position - transform.position).magnitude > missile.travelDistance)
            {
                Destroy(missile.missile);
                missiles.RemoveAt(i);
                GameObject obj = Instantiate(damageArea, missile.missile.transform.position, Quaternion.identity);
                Destroy(obj, 2);
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
        // Remove nulls from destroyed minions
        activeMinions.RemoveAll(minion => minion == null);

        if (activeMinions.Count >= maxMinionCount)
            return;

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 pos = transform.position + (Vector3)dir * spawnRadius;

        GameObject minion = Instantiate(minionPrefab, pos, Quaternion.identity);
        activeMinions.Add(minion); 
    }

    public void OnDeath()
    {
        foreach (var minion in activeMinions)
        {
            minion.GetComponent<Minion>().BecomeGood();
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