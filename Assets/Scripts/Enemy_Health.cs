using UnityEngine;
using System;

public class Enemy_Health : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    public GameObject gravestonePrefab;
    public Vector3 gravestoneOffset = new Vector3(0f, -0.2f, 0f);

    // Event, das beim Tod ausgelöst wird
    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Vector3 spawnPos = transform.position + gravestoneOffset;
            Instantiate(gravestonePrefab, spawnPos, Quaternion.identity);

            OnDeath?.Invoke(); // <-- wichtig!
            Destroy(gameObject);
        }
    }
}
