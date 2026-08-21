using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    enum IsTouchingWall {FALSE, LEFT, RIGHT};
    public Rigidbody2D rb;

    Animator animator;

    private bool moveLeftInput;
    private bool moveRightInput;
    private bool jumpInput;
    public bool clickInput;

    private WebLogic webLogic;
    private bool isTouchingGround = false;
    private IsTouchingWall isTouchingWall = IsTouchingWall.FALSE;
    private bool jumping = false;

    public float moveVelocity = 35;
    public float jumpVelocity = 200;
    public float fallVelocity = 650;
    public float maxSpeed = 15;

    void Start()
    {
        webLogic = FindAnyObjectByType<WebLogic>();
        animator = GetComponentInParent<Animator>();
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
        SpriteHandler();

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
        else if (jumpInput && isTouchingWall!=0)
        {
            if (rb.linearVelocityY < 0) 
            {
                rb.linearVelocityY = 0;
            }

            rb.AddForceY(jumpVelocity * Time.deltaTime, ForceMode2D.Impulse);


            if (isTouchingWall == IsTouchingWall.LEFT)
            {
                rb.AddForceX(jumpVelocity * Time.deltaTime/2, ForceMode2D.Impulse);
            }
            else if (isTouchingWall == IsTouchingWall.RIGHT)
            {
                rb.AddForceX(-jumpVelocity * Time.deltaTime/2, ForceMode2D.Impulse);
            }

            isTouchingWall = IsTouchingWall.FALSE;
        }
        if (rb.linearVelocityY < 0 && !isTouchingGround)
        {
            rb.AddForceY(-fallVelocity * Time.deltaTime, 0);
        }
        
    }

    #region CollisionLogic
    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Ground"))
        {
            isTouchingGround = true;
            jumping = false;
        }
        if (collision.collider.CompareTag("WallLeft"))
        {
            isTouchingWall = IsTouchingWall.LEFT;
        }
        else if (collision.collider.CompareTag("WallRight"))
        {
            isTouchingWall = IsTouchingWall.RIGHT;
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
            isTouchingWall = IsTouchingWall.FALSE;
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

    private void SpriteHandler()
    {

        if (moveRightInput)
        {
            animator.SetBool("standingRight", true);
            animator.SetBool("standingLeft", false);
        }
        else if (moveLeftInput)
        {
            animator.SetBool("standingLeft", true);
            animator.SetBool("standingRight", false);
        }

    }

}