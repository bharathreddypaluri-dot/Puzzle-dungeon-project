using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;          // normal speed
    public float sprintMultiplier = 1.8f; // sprint speed when holding Left Shift

    [Header("Bounds")]
  //  public Vector2 minBounds;
   // public Vector2 maxBounds;

    private Animator anim;
    private Rigidbody2D rb;


    // for physics movement
    private Vector2 moveInput;

    // for animation facing direction
    private Vector2 animDir;
    private Vector2 lastAnimDir = new Vector2(0f, -1f); // default facing down

    private bool moving;
    private bool isSprinting;

    public bool boundarytouched = false;

    public Transform LeftBoundary;
    public Transform RightBoundary;

    public Transform TopBoundary;
    public Transform BottomBoundary;

    public Transform LeftUp;
    public Transform LeftDown;

    public Transform RightUp;
    public Transform RightDown;

    public Transform UpLeft;
    public Transform UpRight;

    public Transform DownLeft;
    public Transform DownRight;


    public float raylength=0.5f;

    public LayerMask wallLayer;

     private bool LeftupHit;
    private bool LeftdownHit;

    private bool RightupHit;
    private bool RightdownHit;

    private bool UpleftHit;
    private bool UprightHit;  

    private bool DownleftHit;
    private bool DownrightHit;

    public float dashDistance = 3f;
    public float dashCooldown = 1f;
    private float lastDashTime;
    Vector2 lastMoveDir= Vector2.right;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetInput();
        Animate();

    if(moveInput != Vector2.zero)
           {
               lastMoveDir = moveInput.normalized;
           }

    
        LeftupHit = Physics2D.Raycast(LeftUp.position, Vector2.left, raylength, wallLayer);
        LeftdownHit = Physics2D.Raycast(LeftDown.position, Vector2.left, raylength, wallLayer);

        RightupHit = Physics2D.Raycast(RightUp.position, Vector2.right, raylength, wallLayer);
        RightdownHit = Physics2D.Raycast(RightDown.position, Vector2.right, raylength, wallLayer);

        UpleftHit = Physics2D.Raycast(UpLeft.position, Vector2.up, raylength, wallLayer);
        UprightHit = Physics2D.Raycast(UpRight.position, Vector2.up, raylength, wallLayer);

        DownleftHit = Physics2D.Raycast(DownLeft.position, Vector2.down, raylength, wallLayer);
        DownrightHit = Physics2D.Raycast(DownRight.position, Vector2.down, raylength, wallLayer);

      //  Debug.DrawRay(LeftUp.position, Vector2.left * raylength, Color.red);
       // Debug.DrawRay(LeftDown.position, Vector2.left * raylength, Color.red);

        if(LeftupHit && LeftdownHit)
        {
            boundarytouched = true;
            rb.MovePosition(new Vector2(LeftBoundary.position.x+1.5f , rb.position.y));

        }
        else if(RightupHit && RightdownHit)
        {
            boundarytouched = true;
            rb.MovePosition(new Vector2(RightBoundary.position.x-1.5f , rb.position.y));

        }
        else if(UpleftHit && UprightHit)
        {
            boundarytouched = true;
            rb.MovePosition(new Vector2(rb.position.x, TopBoundary.position.y-1.5f));

        }
        else if(DownleftHit && DownrightHit)
        {
            boundarytouched = true;
            rb.MovePosition(new Vector2(rb.position.x, BottomBoundary.position.y+1.5f));
        }
        else
        {
            boundarytouched = false;
        }
           
            if(Input.GetKeyDown(KeyCode.Space) && Time.time>= lastDashTime + dashCooldown)
        {
            if(boundarytouched == false)
            {
            rb.MovePosition(rb.position + lastMoveDir * dashDistance);
            }
            else if(boundarytouched == true)
            {
                if(LeftupHit && LeftdownHit)
                {
                    rb.MovePosition(new Vector2(LeftBoundary.position.x+1.5f , rb.position.y));
                }
                else if(RightupHit && RightdownHit)
                {
                    rb.MovePosition(new Vector2(RightBoundary.position.x-1.5f , rb.position.y));
                }
                else if(UpleftHit && UprightHit)
                {
                    rb.MovePosition(new Vector2(rb.position.x, TopBoundary.position.y-1.5f));
                }
                else if(DownleftHit && DownrightHit)
                {
                    rb.MovePosition(new Vector2(rb.position.x, BottomBoundary.position.y+1.5f));
                }
            }
            lastDashTime = Time.time;
        }



    }

void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(LeftUp.position, LeftUp.position + Vector3.left * raylength);
        Gizmos.DrawLine(LeftDown.position, LeftDown.position + Vector3.left * raylength);

          Gizmos.color = Color.red;
        Gizmos.DrawLine(RightUp.position, RightUp.position + Vector3.right * raylength);
        Gizmos.DrawLine(RightDown.position, RightDown.position + Vector3.right * raylength);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(UpLeft.position, UpLeft.position + Vector3.up * raylength);
        Gizmos.DrawLine(UpRight.position, UpRight.position + Vector3.up * raylength);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(DownLeft.position, DownLeft.position + Vector3.down * raylength);
        Gizmos.DrawLine(DownRight.position, DownRight.position + Vector3.down * raylength);
    }




    void FixedUpdate()
    {
        float speed = moveSpeed;
        if (isSprinting)
            speed *= sprintMultiplier;

        // move with diagonal allowed
        rb.linearVelocity = moveInput.normalized * speed;

      
    }

    void GetInput()
    {
        float x = Input.GetAxisRaw("Horizontal"); 
        float y = Input.GetAxisRaw("Vertical");   

        moveInput = new Vector2(x, y);
        moving = moveInput.sqrMagnitude > 0.01f;


           

        // Sprint with Left Shift
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // ----- Direction for animation (snap to 4 directions) -----
        if (moving)
        {
            animDir = moveInput;

            // decide which axis is stronger -> face that way
            if (Mathf.Abs(animDir.x) > Mathf.Abs(animDir.y))
            {
                animDir.y = 0f;
                animDir.x = Mathf.Sign(animDir.x);   // -1 or 1
            }
            else
            {
                animDir.x = 0f;
                animDir.y = Mathf.Sign(animDir.y);   // -1 or 1
            }

            // remember last direction when moving
            lastAnimDir = animDir;
        }
    }

    void Animate()
    {
        Vector2 dirToUse;

        if (moving)
        {
            dirToUse = animDir;
        }
        else
        {
            // not moving -> keep facing last direction
            dirToUse = lastAnimDir;
        }

        anim.SetFloat("X", dirToUse.x);
        anim.SetFloat("Y", dirToUse.y);
        anim.SetBool("Moving", moving);
        anim.SetBool("Sprinting", isSprinting); // ok even if unused in Animator
    }
    
    private void OnCollisionEnter2D(Collision2D collider)
   {
      Debug.Log("Collided with " + collider.gameObject.name);
    
    }
   
    private void OnCollisionExit2D(Collision2D collider)
    {
        Debug.Log("Stopped Colliding with " + collider.gameObject.name);
    }
    
   

 
}


