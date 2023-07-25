using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float movement = 0f;
    public float movementSpeed = 50;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpForce = 700f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    private Path path;
    private int currentWaypoint = 0;
    bool jump = false;

    Animator animator;
    CharacterController2D controller;
    RaycastHit2D isGrounded;
    Seeker seeker;
    Rigidbody2D rb;

    public void OnLanding()
    {
        animator.SetBool("isJumping", false);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<CharacterController2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
        // controller.Move(movement * Time.fixedDeltaTime, false, jump);
        // jump = false;

        // using rigidbody add force
        Vector2 force = new Vector2(movement * movementSpeed * Time.fixedDeltaTime, 0);
        // if jump
        if (jump)
        {
            force.y = jumpForce;
            jump = false;
        }
        rb.AddForce(force);
    }

    void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        movement = 0;

        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // See if colliding with anything
        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);


        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        
        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                jump = true;
                animator.SetBool("isJumping", true);
            }
        }

        // Movement
        movement = directionLookEnabled ? direction.x * movementSpeed : 0;

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

}
