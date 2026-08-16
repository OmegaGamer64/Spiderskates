using UnityEngine;
using UnityEngine.InputSystem;


public class WebLogic : MonoBehaviour
{
    public enum WEB_STATE { IDLE, SHOOTING, SHOT };
   
    Transform player;
    [HideInInspector]
    public PlayerController playerController;
    
    LineRenderer web;
    Vector2 webTarget;
    Vector2 direction;
    public Vector2 offset;
    [HideInInspector]
    public GameObject webEnd;
    [HideInInspector]
    public SpringJoint2D webSpring;
    Rigidbody2D webEndRB;

    public WEB_STATE webState;
    [HideInInspector]
    public GameObject webEndPrefab;
    [Space]
    public float webSpeed = 10f;
    public float maxWebLength = 10f;
    [Tooltip("In percentage")]
    public float defaultDistanceFromWeb = 50f;
    public float maxSpeedWhileShooting = 100;

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
            web.SetPosition(0, player.position+(Vector3)offset);
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

                    Vector2 webDir = (Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())
                        - player.position).normalized;

                    webTarget = (Vector2)player.transform.position+(webDir*maxWebLength);

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

                break;

            case WEB_STATE.SHOT:

                if (playerController.rb.linearVelocityX > maxSpeedWhileShooting)
                {
                    playerController.rb.linearVelocityX = maxSpeedWhileShooting;
                }
                else if (-playerController.rb.linearVelocityX < -maxSpeedWhileShooting)
                {
                    playerController.rb.linearVelocityX = -maxSpeedWhileShooting;
                }
                if (playerController.rb.linearVelocityY > maxSpeedWhileShooting)
                {
                    playerController.rb.linearVelocityY = maxSpeedWhileShooting;
                }
                else if (-playerController.rb.linearVelocityY < -maxSpeedWhileShooting)
                {
                    playerController.rb.linearVelocityY = -maxSpeedWhileShooting;
                }

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

        WebDistanceHandler();

    }

    private void SpawnNextWebSegment()
    {

        webState = WEB_STATE.SHOOTING;

        direction = (webTarget - (Vector2)player.position - offset).normalized;
        Quaternion rotation= new Quaternion();
        rotation.SetFromToRotation(player.position + (Vector3)offset, webTarget);

        webEnd = Instantiate(webEndPrefab, player.position+(Vector3)offset, rotation, transform);
        webEndRB = webEnd.GetComponent<Rigidbody2D>();
        webEndRB.linearVelocity = playerController.rb.linearVelocity;
        webEndRB.AddForce(direction * webSpeed, ForceMode2D.Impulse);
    }

    private void WebDistanceHandler()
    {
        switch (webState)
        {
            case WEB_STATE.SHOOTING:

                if (Vector2.Distance(player.transform.position, webEnd.transform.position) >= maxWebLength)
                {
                    Debug.Log("Max web distance reached");

                    webEndRB.linearVelocity = playerController.rb.linearVelocity+(Physics2D.gravity*webEndRB.gravityScale/3);
                }
                break;

            default:
                break;
        }
        
    }

}
