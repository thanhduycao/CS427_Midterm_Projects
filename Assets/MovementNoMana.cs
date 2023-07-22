using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNoMana : MonoBehaviour
{
    CharacterController2D contrller;

    Animator animator;

    public float movementSpeed;

    public int playerNumber;

    public float playerPower;
    public float currentPower;

    public float movement = 0f;

    bool jump = false;

    public string Sname;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        contrller = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = 0;

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
            jump = true;
            animator.SetBool("isJumping", true);
        }
        if (Input.GetKeyDown(KeyCode.D)) { MusicManager.findMusic(Sname); }
        if (Input.GetKeyDown(KeyCode.A)) { MusicManager.findMusic(Sname); }

        animator.SetFloat("speed", Mathf.Abs(movement));
    }

    public void OnLanding()
    {
        animator.SetBool("isJumping", false);
    }
    void FixedUpdate()
    {
        /* if (Mathf.Abs(movement) > 0)
        {
            contrller.Move(movement * Time.fixedDeltaTime, false, jump);
            jump = false;
        } */
        contrller.Move(movement * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
