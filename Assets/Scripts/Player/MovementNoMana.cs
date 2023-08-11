using Unity.Netcode;
using UnityEngine;

public class MovementNoMana : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private int playerNumber;
    [SerializeField] private float playerPower;
    [SerializeField] private float currentPower;
    [SerializeField] private float movement = 0f;
    [SerializeField] private string Sname;
    [SerializeField] private int extraJumpValue = 1;

    [Header("Metadata")]
    [SerializeField] private float mana = 100f;
    [SerializeField] private float health = 100f;

    [Header("Effects")]
    [Header("Knock back")]
    [SerializeField] private float KBForce;
    [SerializeField] public float KBTotalTime;

    protected PlayerController controller;
    private Animator animator;
    private Rigidbody2D rb;

    private bool jump = false;
    private bool fall = false;
    private bool hit = false;
    private bool onLanded = true;
    private bool doubleJump;
    private int extraJump;

    public float KBCounter = 0;
    public bool KnockFromRight;
    public bool isDialogProcessing = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDialogProcessing)
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
                    // animator.SetBool("isDoubleJump", doubleJump);
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
                // animator.SetBool("isDoubleJump", false);
                if (fall == false && onLanded == false)
                {
                    fall = true;
                    animator.SetBool("isFalling", true);
                }
            }
            //if (Input.GetKeyDown(KeyCode.D)) { MusicManager.findMusic(Sname); }
            //if (Input.GetKeyDown(KeyCode.A)) { MusicManager.findMusic(Sname); }

            animator.SetFloat("speed", Mathf.Abs(movement));
        }
    }

    public void OnLanding()
    {
        // animator.SetBool("isJumping", false);
        if (rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isHit", false);
            onLanded = true;
            fall = false;
        }
    }

    public void OnHit(bool hit)
    {
        animator.SetBool("isHit", hit);
        if (controller.getFacingProperty())
        {
            rb.AddForce(transform.up * 200 + (transform.right * 500) * (-1));
        }
        else
        {
            rb.AddForce(transform.up * 500 + transform.right * 500);
        }
    }

    public bool GetFacingProperty()
    {
        return controller.getFacingProperty();
    }

    public float GetMana()
    {
        return mana;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetMana(float newMana)
    {
        mana = newMana;
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public void ReduceMana(float amount)
    {
        mana -= amount;
    }

    public void ReduceHealth(float amount)
    {
        health -= amount;
    }

    public void IncreaseMana(float amount)
    {
        mana += amount;
    }

    public void IncreaseHealth(float amount)
    {
        health += amount;
    }

    public void checkKnockbackEffect()
    {
        if (KBCounter > 0)
        {
            if (KnockFromRight == true)
            {
                rb.velocity = new Vector2(-KBForce, KBForce);
            }
            if (KnockFromRight == false)
            {
                rb.velocity = new Vector2(KBForce, KBForce);
            }
            KBCounter -= Time.deltaTime;
            animator.SetBool("isHit", true);
        }
    }

    void FixedUpdate()
    {
        controller.Move(movement * Time.fixedDeltaTime, false, jump, doubleJump);
        checkKnockbackEffect();
        jump = false;
        doubleJump = false;
    }
}
