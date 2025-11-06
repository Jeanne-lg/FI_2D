using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    [Header("Angriffseinstellungen")]
    public Transform attackPoint;
    public float weaponRange = 1.2f;
    public LayerMask enemyLayers;
    public int attackDamage = 10;
    public Animator animator;

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Attack();
        }
    }

    public void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("strike1");
    }

    // Wird per Animation Event ausgelöst
    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayers);

        foreach (Collider2D enemy in enemies)
        {
            Enemy_Health health = enemy.GetComponent<Enemy_Health>();
            if (health != null)
            {
                health.ChangeHealth(-attackDamage);
                Debug.Log($"Enemy getroffen: {enemy.name}");
            }
        }
    }

    // Wird am Ende der Attack-Animation ausgelöst
    public void StopAttack()
    {
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
