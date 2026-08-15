using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb;

    private bool moveLeftInput;
    private bool moveRightInput;
    private bool jumpInput;
    public bool clickInput;

    private WebLogic webLogic;
    private bool isTouchingGround = false;
    private bool isTouchingWall = false;
    private bool jumping = false;

    public float moveVelocity = 35;
    public float jumpVelocity = 200;
    public float fallVelocity = 650;
    public float maxSpeed = 15;

    void Start()
    {
        webLogic = FindAnyObjectByType<WebLogic>();
    }

    void Update()
    {
        #region PlayerInputs
        if (InputSystem.actions.FindAction("Move").ReadValue<Vector2>().x < 0)
        {
            moveLeftInput = true;
        }
        else
        {
            moveLeftInput = false;
        }
        
        if (InputSystem.actions.FindAction("Move").ReadValue<Vector2>().x > 0)
        {
            moveRightInput = true;
        }
        else
        {
            moveRightInput = false;
        }

        if (jumpInput && !InputSystem.actions.FindAction("Jump").IsInProgress()&&rb.linearVelocityY>0)
        {
            SlowYVelocity();//FixedUpdate not quick enough to detect
        }

        if (InputSystem.actions.FindAction("Jump").IsPressed())
        {
            jumpInput = true;
        }
        else
        {
            jumpInput = false;
        }

        if (InputSystem.actions.FindAction("Shoot").IsPressed())
        {
            clickInput = true;
        }
        else
        {
            clickInput = false;
        }
        #endregion

        if(webLogic.webState == WebLogic.WEB_STATE.SHOT)
        {
            jumping = false;
        }

    }

    private void FixedUpdate()
    {
        if (moveLeftInput)
        {
            if (rb.linearVelocityX > -maxSpeed)
            {
                rb.AddForceX(-moveVelocity * Time.deltaTime, 0);
            }
            
        }
        if (moveRightInput)
        {
            if (rb.linearVelocityX < maxSpeed)
            {
                rb.AddForceX(moveVelocity * Time.deltaTime, 0);
            }
        }
        if (jumpInput&&isTouchingGround)
        {
            rb.AddForceY(jumpVelocity * Time.deltaTime, ForceMode2D.Impulse);
            jumping = true;
            isTouchingGround = false;
        }
        else if (jumpInput && isTouchingWall)
        {
            rb.AddForceY(jumpVelocity * Time.deltaTime, ForceMode2D.Impulse);
            isTouchingWall = false;
        }
        if (rb.linearVelocityY < 0 && !isTouchingGround)
        {
            rb.AddForceY(-fallVelocity * Time.deltaTime, 0);
        }
        
    }

    #region CollisionLogic
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Ground"))
        {
            isTouchingGround = true;
            jumping = false;
        }
        if (collision.collider.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Ground"))
        {
            isTouchingGround = true;
            jumping = false;
        }
        if (collision.collider.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Ground"))
        {
            Invoke("CoyoteTime", 0.1f);
        }
        if (collision.collider.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }

    }
    #endregion
    private void CoyoteTime()
    {
        isTouchingGround = false;
        if (jumpInput)
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }
    }
    
    private void SlowYVelocity()
    {
        if (!isTouchingGround&&jumping)
        {
            rb.linearVelocityY = 0;
        }
    }
}