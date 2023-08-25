using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public GameObject enemyPrefab;
    public int enemyBuffer = 3;
    public float baseSpawnRate = 2f;
    public float spawnRateRandomness = 0.5f;

    private List<EnemyMover> enemyPool = new List<EnemyMover>();
    private float spawnCooldown = 0f;

    private void Start() {
        InitializeEnemyPool();
    }

    private void Update() {
        if (PlayerSystem.instance.startedGame) {
            if (spawnCooldown <= 0f) {
                SpawnEnemy();
                CalculateSpawnCooldown();
            }

            spawnCooldown -= Time.deltaTime;
        }
    }

    private void InitializeEnemyPool() {
        for (int i = 0; i < enemyBuffer; i++) {
            GameObject enemyObject = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            EnemyMover enemyMover = enemyObject.GetComponent<EnemyMover>();
            enemyMover.gameObject.SetActive(false);
            enemyPool.Add(enemyMover);
        }
    }

    private void SpawnEnemy() {
        EnemyMover availableEnemy = GetAvailableEnemy();
        if (availableEnemy != null) {
            availableEnemy.gameObject.SetActive(true);
            availableEnemy.Spawn();
        }
    }

    private EnemyMover GetAvailableEnemy() {
        foreach (EnemyMover enemyMover in enemyPool) {
            if (!enemyMover.isActive) {
                return enemyMover;
            }
        }
        return null;
    }

    private void CalculateSpawnCooldown() {
        float randomMultiplier = Random.Range(1f - spawnRateRandomness, 1f + spawnRateRandomness);
        spawnCooldown = baseSpawnRate * randomMultiplier;
    }
}
