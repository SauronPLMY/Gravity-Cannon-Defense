using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public float timeBetweenWaves = 3f;
    private float nextWave = 0f;

    public int enemiesPerWave = 4;

    void Update()
    {
        if (Time.time >= nextWave)
        {
            SpawnWave();
            nextWave = Time.time + timeBetweenWaves;
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, sp.position, Quaternion.identity);
        }
    }
}
