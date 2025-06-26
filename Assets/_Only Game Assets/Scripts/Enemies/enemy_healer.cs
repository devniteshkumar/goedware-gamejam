using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;

public class enemy_healer : MonoBehaviour

{
    public int i;
    quaternion rotation;
    Vector2 movedir;
    [SerializeField] GameObject close_enemy;
    [SerializeField] GameObject archer_enemy;

    GameObject current_enemy = null;
    public Stack<GameObject> enemy_to_heal = new Stack<GameObject>();
    // public Stack<GameObject> archer_enemy_to_heal = new Stack<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame 
    void Update()
    {
        if (current_enemy == null)
        {
            if (enemy_to_heal.Count != 0)
            {
                current_enemy = enemy_to_heal.Pop();
            }
            // else if (archer_enemy_to_heal.Count != 0)
            // {
            // current_enemy = archer_enemy_to_heal.Pop();
            //     Debug.Log("a");
            // }
        }
        else
        {
            if (current_enemy.GetComponent<HealthSystem>().currentHealth != current_enemy.GetComponent<HealthSystem>().maxHealth && !current_enemy.GetComponent<HealthSystem>().dead )
            {
                RotateEnemy(current_enemy);
                // Debug.Log("healing");
                current_enemy.GetComponent<HealthSystem>().Heal(10 * Time.deltaTime);
            }
            else
                current_enemy = null;
        }
    }
    void RotateEnemy(GameObject enemy)
    {
        movedir = enemy.transform.position - transform.position;
        movedir.Normalize();
        rotation = Quaternion.LookRotation(Vector3.forward, movedir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200 * Time.deltaTime);
    }


}
