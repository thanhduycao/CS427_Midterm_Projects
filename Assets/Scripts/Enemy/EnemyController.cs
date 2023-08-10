using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyController : MovementController
{
    [Header("Pathfinding")]
    [SerializeField] private Transform target;
    [SerializeField] private float pathUpdateSeconds = 0.5f;
    [SerializeField] private float activateDistance = 50f;

    [Header("Enemy")]
    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private float _speedUp = 10f;
    [SerializeField] private float _radiusScanArea = 1f;
    [SerializeField] private Transform m_Rada;

    [Header("Enemy Area")]
    [SerializeField] private Transform _enemyArea;
    [SerializeField] private float _moveStartX = 0f;
    [SerializeField] private float _moveEndX = 0f;
    [SerializeField] private float _moveStartY = 0f;
    [SerializeField] private float _moveEndY = 0f;

    [Header("Custom Behavior")]
    [SerializeField] bool jumpEnabled = true;
    [SerializeField] private bool directionLookEnabled = true;

    [Header("Physics")]
    [SerializeField] private float movement = 1f;
    [SerializeField] private float nextWaypointDistance = 3f;
    [SerializeField] private float jumpNodeHeightRequirement = 0.8f;

    private Path path;
    private Seeker seeker;
    private Transform m_EnemyTransform;
    private Animator m_Animator;

    private bool jump = false;
    private bool fall = false;
    private bool hit = false;
    private bool onLanded = true;
    private int currentWaypoint = 0;
    private bool isGrounded = false;
    private bool followEnabled = false;

    private bool coroutine = true;

    private void Start()
    {
        if (_enemyArea != null)
        {
            _moveStartY = _enemyArea.localPosition.y - _enemyArea.localScale.y / 2;
            _moveEndY = _enemyArea.localPosition.y + _enemyArea.localScale.y / 2;
            _moveStartX = _enemyArea.localPosition.x - _enemyArea.localScale.x / 2;
            _moveEndX = _enemyArea.localPosition.x + _enemyArea.localScale.x / 2;
        }

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_EnemyTransform = GetComponent<Transform>();
        m_Animator = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        m_Rada.GetComponent<CircleCollider2D>().radius = _radiusScanArea;

        if (IsServer)
            InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnTriggerEnter2DServer(collision.gameObject.transform);
        }
    }

    // [ServerRpc(RequireOwnership = false)]
    public void OnTriggerEnter2DServer(Transform playerTransform)
    {
        if (!IsServer) return;
        // stop scanning
        coroutine = false;

        // start following
        followEnabled = true;
        target = playerTransform;
    }

    private void Update()
    {
        if (!IsServer) return;
        if (TargetInDistance())
        {
            PathFollow();
            Move(movement * (movementSpeed + _speedUp) * Time.deltaTime, false, jump, false);
            jump = false;
        }

        if (coroutine)
        {
            if (m_EnemyTransform.localPosition.x < _moveStartX)
            {
                movement = 1;
                m_EnemyTransform.localPosition = new Vector3(_moveStartX, m_EnemyTransform.localPosition.y, _moveEndY);
            }
            
            if (m_EnemyTransform.localPosition.x > _moveEndX)
            {
                movement = -1;
                m_EnemyTransform.localPosition = new Vector3(_moveEndX, m_EnemyTransform.localPosition.y, _moveEndY);
            }

            Move(movement * movementSpeed * Time.deltaTime, false, false, false);
        }

        if (m_Rigidbody2D.velocity.y < -.3f)
        {
            jump = false;
            m_Animator.SetBool("isJumping", false);
            if (fall == false && onLanded == false)
            {
                fall = true;
                m_Animator.SetBool("isFalling", true);
            }
        }

        m_Animator.SetFloat("speed", Mathf.Abs(movement));
    }

    public void OnLanding()
    {
        // animator.SetBool("isJumping", false);
        if (m_Rigidbody2D.velocity.y < 0)
        {
            m_Animator.SetBool("isFalling", false);
            m_Animator.SetBool("isHit", false);
            onLanded = true;
            fall = false;
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
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - m_Rigidbody2D.position).normalized;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                jump = true;
                m_Animator?.SetBool("isJumping", true);
            }
        }

        // Movement
        movement = directionLookEnabled ? direction.x : 0;

        // Next Waypoint
        float distance = Vector2.Distance(m_Rigidbody2D.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private void UpdatePath()
    {
        if (!followEnabled) return;
        if (TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(m_Rigidbody2D.position, target.position, OnPathComplete);
        }
    }

    private bool TargetInDistance()
    {
        if (!followEnabled) return false;
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