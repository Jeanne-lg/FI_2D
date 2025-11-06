using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_AI : MonoBehaviour
{
    [Header("Allgemeine Bewegung")]
    public float wanderSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float wanderChangeInterval = 3f;
    public LayerMask obstacleLayer;
    public Transform obstacleCheckPoint;
    public float obstacleCheckDistance = 0.5f;

    [Header("Kampfverhalten")]
    public float attackRange = 1.2f;
    public float attackCooldown = 2f;
    public float playerDetectRange = 5f;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;

    private EnemyState2 enemyState2 = EnemyState2.Idle;
    private float attackCooldownTimer;
    private float wanderTimer;
    private Vector2 wanderDirection;
    private int facingDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChooseNewWanderDirection();
    }

    void Update()
    {
        CheckForPlayer();

        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        switch (enemyState2)
        {
            case EnemyState2.Idle:
            case EnemyState2.Wandering:
                Wander();
                break;
            case EnemyState2.Chasing:
                Chase();
                break;
            case EnemyState2.Attacking:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    // === WANDER-LOGIK ===
    void Wander()
    {
        wanderTimer -= Time.fixedDeltaTime;

        // Prüfen, ob Hindernis voraus
        bool hitObstacle = Physics2D.Raycast(obstacleCheckPoint.position, transform.right, obstacleCheckDistance, obstacleLayer);

        if (wanderTimer <= 0 || hitObstacle)
        {
            ChooseNewWanderDirection();
        }

        rb.linearVelocity = new Vector2(wanderDirection.x * wanderSpeed, rb.linearVelocity.y);

        animator.SetBool("isIdle", false);
        animator.SetBool("isChasing", true); // "isChasing" = Laufen-Animation
    }

    void ChooseNewWanderDirection()
    {
        wanderTimer = wanderChangeInterval;
        facingDirection = Random.value > 0.5f ? 1 : -1;
        wanderDirection = new Vector2(facingDirection, 0);

        // Blickrichtung anpassen
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDirection;
        transform.localScale = scale;
    }

    // === CHASEN ===
    void Chase()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;

        // Flip bei Bedarf
        if ((direction.x > 0 && facingDirection == -1) || (direction.x < 0 && facingDirection == 1))
            Flip();

        animator.SetBool("isIdle", false);
        animator.SetBool("isChasing", true);
    }

    void Flip()
    {
        facingDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDirection;
        transform.localScale = scale;
    }

    // === SPIELER-KONTROLLE ===
    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float dist = Vector2.Distance(transform.position, player.position);

            if (dist < attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState2.Attacking);
            }
            else if (dist > attackRange)
            {
                ChangeState(EnemyState2.Chasing);
            }
        }
        else
        {
            ChangeState(EnemyState2.Wandering);
        }
    }

    // === ZUSTANDSWECHSEL ===
    void ChangeState(EnemyState2 newState)
    {
        if (enemyState2 == newState) return;

        animator.SetBool("isIdle", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);

        enemyState2 = newState;

        switch (enemyState2)
        {
            case EnemyState2.Idle:
                animator.SetBool("isIdle", true);
                break;
            case EnemyState2.Wandering:
            case EnemyState2.Chasing:
                animator.SetBool("isChasing", true);
                break;
            case EnemyState2.Attacking:
                animator.SetBool("isAttacking", true);
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
        }
        if (obstacleCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(obstacleCheckPoint.position, obstacleCheckPoint.position + transform.right * obstacleCheckDistance);
        }
    }
}

public enum EnemyState2
{
    Idle,
    Wandering,
    Chasing,
    Attacking
}
