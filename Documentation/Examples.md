# Ludo Pools Examples

This document provides real-world examples of how to use the Ludo Pools system in various scenarios.

## Basic Examples

### Simple Object Pooling

```csharp
using UnityEngine;
using Ludo.Core.Pools.Runtime;

public class SimplePoolExample : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;

    // Use dependency injection to get the pool manager
    [Inject] private IPoolManager _poolManager;

    private void Awake()
    {
        // Preload 10 cubes
        _poolManager.CreatePool(cubePrefab, 10, 100);
    }

    private void Update()
    {
        // Spawn a cube on space key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnCube();
        }
    }

    private void SpawnCube()
    {
        // Get a cube from the pool
        GameObject cube = _poolManager.GetPooledObject(cubePrefab);

        // Position it randomly
        cube.transform.position = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(5f, 10f),
            Random.Range(-5f, 5f)
        );

        // Add a component to return it to the pool after 3 seconds
        var returnComponent = cube.GetComponent<ReturnToPoolAfterDelay>();
        if (returnComponent == null)
        {
            returnComponent = cube.AddComponent<ReturnToPoolAfterDelay>();
        }
        returnComponent.delay = 3f;
    }
}
```

## Game Examples

### Enemy Spawner

This example shows how to use object pooling for spawning enemies in a game:

```csharp
using UnityEngine;
using Ludo.Core.Pools.Runtime;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;

    [Inject] private IPoolManager _poolManager;
    private float _timer;

    private void Awake()
    {
        // Preload each enemy type
        foreach (var prefab in enemyPrefabs)
        {
            _poolManager.CreatePool(prefab, 5, 20);
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            SpawnEnemy();
            _timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // Select a random enemy prefab
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Select a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Get an enemy from the pool
        GameObject enemy = _poolManager.GetPooledObject(prefab);

        // Position and rotate it
        enemy.transform.position = spawnPoint.position;
        enemy.transform.rotation = spawnPoint.rotation;

        // Initialize the enemy
        var enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.Initialize();
        }
    }
}
```

### Projectile System

This example demonstrates using object pooling for a projectile system:

```csharp
using UnityEngine;
using Ludo.Core.Pools.Runtime;

public class ProjectileSystem : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float projectileSpeed = 20f;

    [Inject] private IPoolManager _poolManager;
    private float _fireTimer;

    private void Awake()
    {
        _poolManager.CreatePool(projectilePrefab, 20, 100);
    }

    private void Update()
    {
        _fireTimer -= Time.deltaTime;

        if (Input.GetButton("Fire1") && _fireTimer <= 0)
        {
            FireProjectile();
            _fireTimer = fireRate;
        }
    }

    private void FireProjectile()
    {
        // Get a projectile from the pool
        GameObject projectile = _poolManager.GetPooledObject(projectilePrefab);

        // Position and rotate it
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;

        // Add velocity
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * projectileSpeed;
        }

        // Make sure it has a component to return to the pool
        var returnComponent = projectile.GetComponent<ReturnToPoolAfterDelay>();
        if (returnComponent == null)
        {
            returnComponent = projectile.AddComponent<ReturnToPoolAfterDelay>();
        }
        returnComponent.delay = 5f; // Return after 5 seconds if it doesn't hit anything
    }
}
```

## Real-World Examples from Galacron

### Horizontal Spaceship Spawner

This example is based on the actual implementation in the Galacron game:

```csharp
using UnityEngine;
using Ludo.Core.Pools.Runtime;

public class HorizontalSpaceshipSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject spaceshipPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDelay = 5f;
    [SerializeField] private float maxSpawnDelay = 15f;
    [SerializeField] private float spawnDelayDecreasePerLevel = 0.5f;
    [SerializeField] private float minSpawnDelayLimit = 3f;

    [Inject] private IPoolManager _poolManager;

    private int _currentLevel = 0;
    private bool _canSpawn = false;
    private Coroutine _spawnCoroutine;

    private void Start()
    {
        GameEventHub.Bind(this);
    }

    private void OnDestroy()
    {
        GameEventHub.Unbind(this);
        StopSpawning();
    }

    [OnGameEvent(typeof(NewLevelEvent))]
    [Preserve]
    private void OnNewLevel(NewLevelEvent evt)
    {
        _currentLevel = evt.Level;
    }

    [OnGameEvent(typeof(SpawnCompleteEvent))]
    [Preserve]
    private void OnSpawnComplete(SpawnCompleteEvent evt)
    {
        StartSpawning();
    }

    [OnGameEvent(typeof(LevelCompleteEvent))]
    [Preserve]
    private void OnLevelComplete(LevelCompleteEvent evt)
    {
        StopSpawning();
    }

    private void StartSpawning()
    {
        _canSpawn = true;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }
        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void StopSpawning()
    {
        _canSpawn = false;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private void SpawnSpaceship()
    {
        if (spaceshipPrefab == null) return;

        // Randomly decide direction
        bool moveLeftToRight = Random.value > 0.5f;

        // Get spaceship from pool
        GameObject spaceship = _poolManager.GetPooledObject(spaceshipPrefab);

        // Initialize the spaceship
        var spaceshipComponent = spaceship.GetComponent<HorizontalSpaceship>();
        if (spaceshipComponent != null)
        {
            spaceshipComponent.Initialize(moveLeftToRight);
        }
    }
}
```
