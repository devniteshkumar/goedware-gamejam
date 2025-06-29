using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float SmoothSpeed;
    void LateUpdate()
    {
        if(player)  transform.position = Vector3.Lerp(transform.position, player.position + new Vector3(0, 0, -10), SmoothSpeed);
    }
}
