using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class one_line_enemy : MonoBehaviour
{
    public GameObject player;
    Vector3 rotation;
    [SerializeField] float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        rotation = player.transform.position - transform.position;
        rotation.Normalize();

        
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }
    void move()
    {
        transform.Translate(rotation * speed * Time.deltaTime);
        
    }
}
