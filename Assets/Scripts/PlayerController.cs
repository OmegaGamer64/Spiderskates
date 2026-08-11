using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    private bool moveLeftInput;
    private bool moveRightInput;
    private bool jumpInput;
    public bool clickInput;

    private bool isTouchingGround = false;

    public float moveVelocity = 35;
    public float jumpVelocity = 200;
    public float fallVelocity = 650;

    // Update is called once per frame
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

    }

    private void FixedUpdate()
    {
        if (moveLeftInput)
        {
            rb.AddForceX(-moveVelocity * Time.deltaTime, 0);
        }
        if (moveRightInput)
        {
            rb.AddForceX(moveVelocity * Time.deltaTime, 0);
        }
        if (jumpInput&&isTouchingGround)
        {
            rb.AddForceY(jumpVelocity * Time.deltaTime, ForceMode2D.Impulse);
            isTouchingGround = false;
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
        }
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Ground"))
        {
            isTouchingGround = true;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Ground"))
        {
            Invoke("CoyoteTime", 0.25f);
        }

    }
    #endregion
    private void CoyoteTime()
    {
        isTouchingGround = false;
    }
    
    private void SlowYVelocity()
    {
        rb.linearVelocityY = 0;//rb.linearVelocityY / 3f;
    }
}