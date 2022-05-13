using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    // Another singleton
    public static GameHandler Instance { get; private set; }
    private void Awake() => Instance = this;

    // Time in seconds before another enemy spawns
    public float spawnInterval = 5;

    // How fast the interval decreases
    public float intervalDecreaseRate = 0.05f;

    // Minimum spawn interval
    public float minSpawnInterval = 0.5f;

    // What to spawn in as an enemy
    public GameObject enemyPrefab;

    // Elapsed time since enemy spawn
    public float elapsed = 0;

    public void Update()
    {
        spawnInterval -= Time.deltaTime * intervalDecreaseRate;
        spawnInterval = Mathf.Max(spawnInterval, minSpawnInterval);

        elapsed += Time.deltaTime;
        if(elapsed >= spawnInterval)
        {
            Vector3 attemptedSpot = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            UnityEngine.AI.NavMeshHit hit;
            if(UnityEngine.AI.NavMesh.SamplePosition(attemptedSpot, out hit, 100, UnityEngine.AI.NavMesh.AllAreas))
            {
                attemptedSpot = hit.position + Vector3.up;
                var spawned = Instantiate(enemyPrefab);
                spawned.transform.position = attemptedSpot;
                elapsed = 0;
            }
        }
    }
}
