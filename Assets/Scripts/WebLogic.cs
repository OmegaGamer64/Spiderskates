using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WebLogic : MonoBehaviour
{
    Transform player;
    PlayerController playerController;
    
    LineRenderer web;
    Vector2 webTarget;
    Vector2 webEndPoint;
    int count = 0;

    public GameObject webPrefab;
    [Space]
    public float webDelay = 0.025f;
    public float webSpeed = 0.025f;

    private bool isShooting = false;
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        player = playerController.GetComponentInParent<Transform>();
    }
    void FixedUpdate()
    {
        if (playerController.clickInput&&!isShooting)
        {
            webTarget = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            isShooting = true;

            #region WebSettings
            web = gameObject.AddComponent<LineRenderer>();
            web.material = new Material(Shader.Find("Sprites/Default"));
            web.startColor = Color.white;
            web.endColor = Color.white;
            web.startWidth = 0.25f;
            web.endWidth = 0.25f;
            web.positionCount = 2;
            #endregion

            web.SetPosition(0, player.position);
            web.SetPosition(1, player.position);

            count = 0;
            Invoke("SpawnNextWebSegment", webDelay);
            
        }
        if (!playerController.clickInput)
        {
            isShooting = false;
            Destroy(web);
            count = 0;
        }
        if (isShooting)
        {
            web.SetPosition(0, player.position);
        }
    }

    void SpawnNextWebSegment()
    {
        count++;
        if (count < 20 && isShooting) 
        {
            float x = player.position.x + ((webTarget.x - player.position.x) / (20 - count));
            float y = player.position.y + (webTarget.y - player.position.y) / (20 - count);
            webEndPoint = new Vector2(x, y);

            web.SetPosition(1, webEndPoint);
            Invoke("SpawnNextWebSegment", webSpeed);
        }
        else
        {
            return;
        }
    }
}
