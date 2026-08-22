using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class WebLogic : MonoBehaviour
{
    public enum WEB_STATE { IDLE, SHOOTING, SHOT, GRAPPLING };
   
    Transform player;
    [HideInInspector]
    public PlayerController playerController;
    
    LineRenderer web;
    Vector2 webTarget;
    Vector2 direction;
    public Vector2 playerOffset;
    public Vector2 webOffset;
    [HideInInspector]
    public GameObject webEnd;
    [HideInInspector]
    public SpringJoint2D webSpring;
    Rigidbody2D webEndRB;
    [HideInInspector]
    public GameObject grappledEnemy;
    public WEB_STATE webState;
    [HideInInspector]
    public GameObject webEndPrefab;
    Material webTexture;
    [Space]
    public float webSpeed = 10f;
    public float maxWebLength = 10f;
    public float minWebLength = 1f;
    [Tooltip("In percentage")]
    public float defaultDistanceFromWeb = 50f;
    public float maxSpeedWhileShooting = 100;

    void Start()
    {
        webState = WEB_STATE.IDLE;
        playerController = FindAnyObjectByType<PlayerController>();
        player = playerController.GetComponentInParent<Transform>();
        webEndPrefab = Resources.Load<GameObject>("Prefabs/WebEnd");
        webTexture = Resources.Load<Material>("Sprites/WebTexture");
    }

    void LateUpdate()
    {
        if (web != null)
        {
            web.SetPosition(1, player.position+(Vector3)playerOffset);
            web.SetPosition(0, webEnd.transform.position + (Vector3)webOffset);
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
                    web.startWidth = 2f;
                    web.endWidth = 2f;
                    web.positionCount = 2;
                    web.textureMode = LineTextureMode.Tile;
                    web.material = webTexture;
                    web.textureScale = new Vector2(.5f, 1f);
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

            case WEB_STATE.GRAPPLING:

                webEnd.GetComponent<SpriteRenderer>().enabled = false;
                GrappleHandler();

                break;

            default:
                break;
        }
        

    }
    private void Update()
    {

        if (!playerController.clickInput)
        {
            if(webState == WEB_STATE.GRAPPLING)
            {
                grappledEnemy = null;
            }
            webState = WEB_STATE.IDLE;
            Destroy(web);
            Destroy(webEnd);
            
            
        }

        WebDistanceHandler();

    }

    private void SpawnNextWebSegment()
    {

        webState = WEB_STATE.SHOOTING;

        direction = (webTarget - (Vector2)player.position - playerOffset).normalized;

        webEnd = Instantiate(webEndPrefab, player.position+(Vector3)playerOffset, Quaternion.identity, transform);
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

    private void GrappleHandler()
    {

        Vector2 mousePos = new Vector2(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).x,
                                       Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).y);

        Vector2 webDir = (Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())
                        - player.position).normalized;

        if (Vector2.Distance(player.position, mousePos) >= maxWebLength)
        {

            
            mousePos = (Vector2)player.transform.position + (webDir * maxWebLength);

        }
        if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), mousePos) <= minWebLength)
        {
            Debug.Log("Min Web Length Reached");
            webDir = (player.position - Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())).normalized;
            mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) + (webDir * minWebLength);

        }
        grappledEnemy.GetComponent<Rigidbody2D>().MovePosition(mousePos);

        Vector2 enemyPos = new Vector2(grappledEnemy.transform.position.x,
                                       grappledEnemy.transform.position.y);
        webEndRB.MovePosition(enemyPos);

    }

}
