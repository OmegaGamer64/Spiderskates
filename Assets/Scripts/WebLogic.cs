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
    int segments = 20;

    public GameObject webPrefab;
    [Space]
    public float webDelay = 0.025f;
    public float webSpeed = 0.025f;
    public float maxWebLength = 10f;
    public int minSegments = 3;
    public int maxSegments = 20;

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

            segments = (int)(maxSegments * Mathf.Min(1, Vector2.Distance(player.position, webTarget) / maxWebLength)); ;
            if (segments <= minSegments)
            {
                return;
            }

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

        if (count <= segments && isShooting) 
        {
            Vector2 lineWeWant = webTarget - (Vector2)player.position;
            float x = player.position.x + (lineWeWant.x / segments * count);
            float y = player.position.y + (lineWeWant.y / segments * count);
            webEndPoint = new Vector2(x, y);

            float webLength = Vector2.Distance(player.position, webEndPoint);
            

            if (webLength > maxWebLength)
            {
                Debug.Log("Max web length reached");
                webEndPoint = Vector2.MoveTowards(webEndPoint, player.position,
                    Mathf.Abs(webLength - maxWebLength));
                count = segments;
            }

            web.SetPosition(1, webEndPoint);
            Invoke("SpawnNextWebSegment", webSpeed);
        }
        else
        {
            count = 0;
            return;
        }
    }
}
