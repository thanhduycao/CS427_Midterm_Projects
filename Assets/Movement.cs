using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    CharacterController2D contrller;

    Animator animator;

    public float movementSpeed;

    public int playerNumber;

    public float playerPower;
    public float currentPower;

    public float movement;

    public string Sname;
    public void Start()
    {
        animator = GetComponent<Animator>();
        contrller = GetComponent<CharacterController2D>();
    }
    private void Update()
    {
        if (playerNumber == CameraController2.currentPlayer)
        {
            if (gameObject.GetComponent<ReduceMana>().currentMana > 0)
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
                if (Input.GetKeyDown(KeyCode.D)) {gameObject.GetComponent<ReduceMana>().spendMana(20); MusicManager.findMusic(Sname); }
                if (Input.GetKeyDown(KeyCode.A)) { gameObject.GetComponent<ReduceMana>().spendMana(20); MusicManager.findMusic(Sname); }
                if (Mathf.Abs(movement) > 0)
                {
                    contrller.Move(movement * Time.deltaTime, false, false);
                    
                }
                if (gameObject.GetComponent<ReduceMana>().currentMana == 0) movement = 0;

                animator.SetFloat("speed", Mathf.Abs(movement));
            }
        }
    }
}
