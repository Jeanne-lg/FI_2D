using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy_RangedCombat : MonoBehaviour
{
    [Header("Projektil / Firepoint")]
    public GameObject projectilePrefab;
    public Transform firePoint;                // Child-Transform an der Mündung
    public float projectileSpeed = 6f;

    [Header("Angriff")]
    public float attackRange = 6f;
    public float attackCooldown = 2f;
    public LayerMask playerLayer;

    private float attackTimer;
    private Transform player;
    private Animator animator;

    // Merke Ausgangs-X-Offset des firePoints, damit wir beim Flip korrekt spiegeln
    private float firePointInitialLocalX;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (firePoint != null)
            firePointInitialLocalX = firePoint.localPosition.x;
    }

    void Update()
    {
        // Spieler in Reichweite suchen
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;

            // Flip so das Sprite zum Spieler schaut
            HandleFlipTowards(player.position);

            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    private void HandleFlipTowards(Vector3 targetPosition)
    {
        if (targetPosition == null) return;

        // gewünschte Richtung: +1 = rechts, -1 = links
        float dir = Mathf.Sign(targetPosition.x - transform.position.x);
        if (dir == 0) return;

        // setze localScale.x so, dass Sprite nach dir zeigt
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;

        // firePoint muss gespiegelt werden (sofern es ein Child ist)
        if (firePoint != null)
        {
            Vector3 fpLocal = firePoint.localPosition;
            fpLocal.x = Mathf.Abs(firePointInitialLocalX) * dir;
            firePoint.localPosition = fpLocal;
        }
    }

    private void Attack()
    {
        if (player == null || projectilePrefab == null || firePoint == null)
            return;

        animator?.SetBool("isAttacking", true);

        // Zielrichtung (Top-Down: ziel direkt auf Spieler: Y wird berücksichtigt)
        Vector2 direction = (player.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // optional: Projektil ausrichten
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(firePoint.position, 0.12f);
        }
    }
}
