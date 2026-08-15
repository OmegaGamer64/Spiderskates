using UnityEngine;
using UnityEngine.InputSystem;


public class WebLogic : MonoBehaviour
{
    public enum WEB_STATE { IDLE, INPUT, SHOOTING, SHOT };
   
    Transform player;
    PlayerController playerController;
    
    LineRenderer web;
    Vector2 webTarget;
    Vector2 direction;
    GameObject webEnd;
    SpringJoint2D webSpring;
    float webSpringDistanceOriginal;
    Rigidbody2D webEndRB;

    public WEB_STATE webState;
    public GameObject webEndPrefab;
    [Space]
    public float webSpeed = 10f;
    public float maxWebLength = 10f;
    public float playerSpeedTowardsWeb = 30f;

    void Start()
    {
        webState = WEB_STATE.IDLE;
        playerController = FindAnyObjectByType<PlayerController>();
        player = playerController.GetComponentInParent<Transform>();
        webEndPrefab = Resources.Load<GameObject>("Prefabs/WebEnd");
    }

    void LateUpdate()
    {
        if (web != null)
        {
            web.SetPosition(0, player.position);
            web.SetPosition(1, webEnd.transform.position);
        }
    }
    void FixedUpdate()
    {
        switch (webState)
        {
            case WEB_STATE.IDLE:
                if (playerController.clickInput && webState == WEB_STATE.IDLE)
                {
                    webState = WEB_STATE.INPUT;

                    webTarget = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

                    #region WebSettings
                    web = gameObject.AddComponent<LineRenderer>();
                    web.material = new Material(Shader.Find("Sprites/Default"));
                    web.startColor = Color.white;
                    web.endColor = Color.white;
                    web.startWidth = 0.25f;
                    web.endWidth = 0.25f;
                    web.positionCount = 2;
                    #endregion

                    SpawnNextWebSegment();

                }
                break;
            case WEB_STATE.SHOOTING:

                FindWebHit();
                break;

            case WEB_STATE.SHOT:

                WebDistanceHandler();
                break;

            default:
                break;
        }

    }
    private void Update()
    {

        if (!playerController.clickInput)
        {
            webState = WEB_STATE.IDLE;
            Destroy(web);
            Destroy(webEnd);
        }

        if (webState == WEB_STATE.SHOOTING)
        {
            webEndRB.AddForce(direction * webSpeed * Time.deltaTime);
        }

    }

    private void SpawnNextWebSegment()
    {

        webState = WEB_STATE.SHOOTING;

        direction = webTarget - (Vector2)player.position;
        Quaternion rotation= new Quaternion();
        rotation.SetFromToRotation((Vector2)player.position, webTarget);

        webEnd = Instantiate(webEndPrefab, player.transform.position, rotation, transform);
        webEndRB = webEnd.GetComponent<Rigidbody2D>();
        webEndRB.linearVelocity = playerController.rb.linearVelocity;
        webEndRB.AddForce(direction * webSpeed, ForceMode2D.Impulse);
    }

    private bool FindWebHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(webEnd.transform.position, webEnd.transform.position, 0);

        if (hit && !hit.collider.CompareTag("Player"))
        {
            webState = WEB_STATE.SHOT;

            webEndRB.constraints = RigidbodyConstraints2D.FreezeAll;


            webSpring = webEnd.GetComponent<SpringJoint2D>();
            webSpring.enabled = true;
            webSpring.connectedBody = playerController.rb;
            webSpringDistanceOriginal = Vector2.Distance(player.transform.position, webEnd.transform.position)/playerSpeedTowardsWeb;


            return true;
        }
        return false;
    }

    private void WebDistanceHandler()
    {
        switch (webState)
        {
            case WEB_STATE.SHOOTING:

                break;

            case WEB_STATE.SHOT:

                if (Vector2.Distance(player.transform.position, webEnd.transform.position) <= webSpring.distance)
                {
                    webSpringDistanceOriginal = webSpring.distance;
                    webSpring.distance = 0;
                }
                else if (Vector2.Distance(player.transform.position, webEnd.transform.position) / playerSpeedTowardsWeb > webSpringDistanceOriginal)
                {
                    webSpring.distance = webSpringDistanceOriginal;
                }
                break;

            default:
                break;
        }
        
    }

}
