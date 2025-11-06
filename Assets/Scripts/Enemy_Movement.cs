using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{
    public float speed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float playerDetectRange = 5f;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCooldownTimer;
    private int facingDirection = 1; 
    private EnemyState enemyState;

    
    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;

    void Start()
    {
        // Fehlerpr�fung ist immer gut
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D fehlt am GameObject " + gameObject.name);
        }
    }

    // FixedUpdate ist besser f�r die Physik-Manipulation
    void Update()
    {
        CheckForPlayer();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        // 'isChasing' == true kann zu 'isChasing' verk�rzt werden
        if (enemyState == EnemyState.Chasing)
        {
      Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
           rb.linearVelocity = Vector2.zero;
        }
    }
    void Chase()
    {
         if (player.position.x > transform.position.x && facingDirection == -1 ||
                player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        // überprüfen, ob der Spieler existiert, bevor man versucht, ihn zu verfolgen
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed; // velocity ist besser als linearVelocity
        }
    }

    void Flip()
    {
               facingDirection *= -1;
         transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

    }

    
    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;

            if (Vector2.Distance(transform.position, player.position) < attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if (Vector2.Distance(transform.position, player.position) > attackRange)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }


    void ChangeState(EnemyState newState)
    {
        if(enemyState == EnemyState.Idle)
        animator.SetBool("isIdle", false);
        else if(enemyState == EnemyState.Chasing)
            animator.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            animator.SetBool("isAttacking", false);

        enemyState = newState;

        if (enemyState == EnemyState.Idle)
            animator.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            animator.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            animator.SetBool("isAttacking", true);
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}