using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;          // normal speed
    public float sprintMultiplier = 1.8f; // sprint speed when holding Left Shift

    [Header("Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Animator anim;
    private Rigidbody2D rb;

    // for physics movement
    private Vector2 moveInput;

    // for animation facing direction
    private Vector2 animDir;
    private Vector2 lastAnimDir = new Vector2(0f, -1f); // default facing down

    private bool moving;
    private bool isSprinting;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetInput();
        Animate();
    }

    void FixedUpdate()
    {
        float speed = moveSpeed;
        if (isSprinting)
            speed *= sprintMultiplier;

        // move with diagonal allowed
        rb.linearVelocity = moveInput.normalized * speed;

        // clamp position
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, minBounds.x, maxBounds.x);
        p.y = Mathf.Clamp(p.y, minBounds.y, maxBounds.y);
        transform.position = p;
    }

    void GetInput()
    {
        float x = Input.GetAxisRaw("Horizontal"); // A/D, Left/Right
        float y = Input.GetAxisRaw("Vertical");   // W/S, Up/Down

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
}


