using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public Vector2 spawnArea = new Vector2(8f, 4f);
    
    private float _spawnTimer;

    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        
        if (_spawnTimer <= 0f)
        {
            SpawnEnemy();
            _spawnTimer = spawnInterval;
        }
    }
    
    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        
        Vector2 spawnPos = new Vector2(
            transform.position.x + Random.Range(-spawnArea.x, spawnArea.x),
            transform.position.y + Random.Range(-spawnArea.y, spawnArea.y)
        );
        
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}