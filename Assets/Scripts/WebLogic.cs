using UnityEngine;
using UnityEngine.InputSystem;

public class WebLogic : MonoBehaviour
{
    Transform player;
    PlayerController playerController;
    
    LineRenderer web;
    Vector2 webTarget;
    Vector2 webEndPoint;
    GameObject webEndCollider;
    SpringJoint2D webSpring;
    float webSpringDistanceOriginal;


    int count = 0;
    int segments = 20;

    public GameObject webEndPrefab;
    [Space]
    public float webSpeed = 0.025f;
    public float maxWebLength = 10f;
    public float webGravity = 9.8f;
    public int minSegments = 3;
    public int maxSegments = 20;
    public float playerSpeedTowardsWeb = 30f;

    private bool isShooting = false;
    private bool hasShot = false;
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        player = playerController.GetComponentInParent<Transform>();
        webEndPrefab = Resources.Load<GameObject>("Prefabs/WebEnd");
    }
    void FixedUpdate()
    {
        
        if (playerController.clickInput&&!isShooting&&!hasShot)
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
            SpawnNextWebSegment();
            
        }

        if (!playerController.clickInput)
        {
            isShooting = false;
            hasShot = false;
            Destroy(web);
            Destroy(webEndCollider);
            count = 0;
        }

        if (isShooting)
        {

            web.SetPosition(1, (Vector2)web.GetPosition(1) - new Vector2(0, webGravity * Time.deltaTime));//slowly sinking

            webEndPoint = web.GetPosition(1);
            

            FindWebHit();

        }

        if (hasShot)
        {
            web.SetPosition(0, player.position);
            WebDistanceHandler();
        }
    }

    private void Update()
    {

        if (isShooting)
        {
            web.SetPosition(0, player.position);
        }
    }

    private void SpawnNextWebSegment()
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

            if (FindWebHit())
            {
                return;
            }

            web.SetPosition(1, webEndPoint);
            Invoke("SpawnNextWebSegment", webSpeed);
        }
        else
        {
            count = 0;
            Destroy(webEndCollider);
            return;
        }
    }

    private bool FindWebHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(webEndPoint, webEndPoint, 0);

        if (hit && !hit.collider.CompareTag("Player"))
        {
            isShooting = false;
            hasShot = true;

            webEndCollider = Instantiate(webEndPrefab, webEndPoint, Quaternion.identity, transform);
            webSpring = webEndCollider.GetComponent<SpringJoint2D>();
            webSpring.connectedBody = playerController.rb;
            webSpringDistanceOriginal = Vector2.Distance(player.transform.position, webEndPoint)/playerSpeedTowardsWeb;


            return true;
        }
        return false;
    }

    private void WebDistanceHandler()
    {
        Debug.Log(webSpring.distance);
        if (Vector2.Distance(player.transform.position, webEndPoint) <= webSpring.distance )
        {
            webSpringDistanceOriginal = webSpring.distance;
            webSpring.distance = 0;
        }
        else if(Vector2.Distance(player.transform.position, webEndPoint) / playerSpeedTowardsWeb> webSpringDistanceOriginal)
        {
            webSpring.distance = webSpringDistanceOriginal;
        }
    }
}
