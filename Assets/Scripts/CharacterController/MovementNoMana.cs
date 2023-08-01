using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNoMana : MonoBehaviour
{
    CharacterController2D contrller;

    Animator animator;

    Rigidbody2D rb;

    public float movementSpeed;

    public int playerNumber;

    public float playerPower;
    public float currentPower;

    public float movement = 0f;

    public int extraJumpValue;


    bool jump = false;
    bool fall = false;
    bool onLanded = true;
    private bool doubleJump;
    private int extraJump;

    public string Sname;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        contrller = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = 0;
        if (onLanded == true)
        {
            extraJump = extraJumpValue;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement = movementSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement = -movementSpeed;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (extraJump > 0 && onLanded == false)
            {
                doubleJump = true;
                animator.SetBool("isJumping", false);
                animator.SetBool("isDoubleJump", doubleJump);
                extraJump--;
            }
            else
            {
                jump = true;
                onLanded = false;
                animator.SetBool("isJumping", true);
            }
        }
        if (rb.velocity.y < -.3f)
        {
            jump = false;
            animator.SetBool("isJumping", false);
            animator.SetBool("isDoubleJump", false);
            if (fall == false && onLanded == false)
            {
                fall = true;
                animator.SetBool("isFalling", true);
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) { MusicManager.findMusic(Sname); }
        if (Input.GetKeyDown(KeyCode.A)) { MusicManager.findMusic(Sname); }

        animator.SetFloat("speed", Mathf.Abs(movement));
    }

    public void OnLanding()
    {
        // animator.SetBool("isJumping", false);
        if (rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", false);
            onLanded = true;
            fall = false;
        }
    }

    public void OnHit(bool hit)
    {
        animator.SetBool("isHit", hit);
        if (contrller.getFacingProperty())
        {
            rb.AddForce(transform.up * 200 + (transform.right * 500) * (-1));
        }
        else
        {
            rb.AddForce(transform.up * 500 + transform.right * 500);
        }
    }

    void FixedUpdate()
    {
        contrller.Move(movement * Time.fixedDeltaTime, false, jump, doubleJump);
        jump = false;
        doubleJump = false;
    }
}