using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 3f;
    public LayerMask playerLayer;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Wenn das Projektil den Spieler trifft
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Player_Health player = collision.GetComponent<Player_Health>();
            if (player != null)
            {
                player.ChangeHealth(-damage);
            }

            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            // Optional: zerstört sich auch bei Wänden
            Destroy(gameObject);
        }
    }
}
