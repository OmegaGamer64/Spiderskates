using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float xLock;
    public float yLock = 3;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
    }
    void LateUpdate()
    {
        //x camera movment
        transform.position = new Vector3(player.position.x + offset.x, offset.y,offset.z);
    }
}
