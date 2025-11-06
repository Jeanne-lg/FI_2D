using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Einstellungen")]
    public GameObject enemyPrefab;           // Der Gegnertyp, der gespawnt werden soll
    public Transform[] spawnPoints;          // Orte, an denen Gegner erscheinen können
    public float spawnInterval = 5f;         // Zeit zwischen Spawns
    public int maxEnemies = 10;              // Maximalzahl an Gegnern gleichzeitig
    public bool endless = true;              // Ob nach Zerstörung neue gespawnt werden sollen

    private int currentEnemyCount = 0;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (endless)
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
                currentEnemyCount++;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Keine Spawnpunkte zugewiesen!");
            return;
        }

        // Zufälligen Spawnpunkt wählen
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Gegner erzeugen
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Listener: wenn Gegner stirbt, Zähler verringern
        Enemy_Health health = enemy.GetComponent<Enemy_Health>();
        if (health != null)
        {
            health.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath()
    {
        currentEnemyCount--;
        if (currentEnemyCount < 0) currentEnemyCount = 0;
    }
}
