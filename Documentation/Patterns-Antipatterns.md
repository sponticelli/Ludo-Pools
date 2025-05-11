# Ludo Pools: Patterns and Anti-Patterns

This document outlines recommended patterns and anti-patterns when using the Ludo Pools system.

## Recommended Patterns

### 1. Preload Pools for Known Objects

Preload pools for objects that you know will be used frequently to avoid performance hitches during gameplay:

```csharp
private void Awake()
{
    // Preload 20 bullets with a maximum of 100
    PoolManager.Instance.CreatePool(bulletPrefab, 20, 100);
    
    // Preload 10 enemies of each type
    foreach (var enemyPrefab in enemyPrefabs)
    {
        PoolManager.Instance.CreatePool(enemyPrefab, 10, 50);
    }
}
```

### 2. Use Dependency Injection

Use dependency injection to access the pool manager, which makes your code more testable:

```csharp
[Inject] private IPoolManager _poolManager;

// Instead of:
// private PoolManager _poolManager;
// _poolManager = PoolManager.Instance;
```

### 3. Add ReturnToPoolAfterDelay for Temporary Objects

For objects with a predictable lifetime, add the ReturnToPoolAfterDelay component:

```csharp
// For effects, bullets, or temporary objects
GameObject effect = _poolManager.GetPooledObject(effectPrefab);
var returnComponent = effect.GetComponent<ReturnToPoolAfterDelay>();
if (returnComponent == null)
{
    returnComponent = effect.AddComponent<ReturnToPoolAfterDelay>();
}
returnComponent.delay = 2.0f;
```

### 4. Reset Object State When Spawning

Always reset the state of pooled objects when retrieving them from the pool:

```csharp
GameObject enemy = _poolManager.GetPooledObject(enemyPrefab);

// Reset state
var enemyComponent = enemy.GetComponent<Enemy>();
enemyComponent.ResetState();
enemyComponent.Health = enemyComponent.MaxHealth;
enemyComponent.IsActive = true;
```

### 5. Use Extension Methods for Cleaner Code

Use the provided extension methods for cleaner, more readable code:

```csharp
// Instead of:
// _poolManager.ReturnPooledObject(gameObject);

// Use:
gameObject.ReturnToPool();
```

### 6. Implement OnDisable for Safety

Implement OnDisable to return objects to the pool when they're disabled:

```csharp
private void OnDisable()
{
    // Cancel any ongoing actions
    CancelInvoke();
    StopAllCoroutines();
    
    // Return to pool if we're not quitting the application
    if (!gameObject.scene.isLoaded) return; // Skip during scene unload
    
    gameObject.ReturnToPool();
}
```

## Anti-Patterns

### 1. Destroying Pooled Objects Directly

Never destroy pooled objects directly with Destroy():

```csharp
// DON'T DO THIS:
Destroy(pooledObject);

// DO THIS INSTEAD:
pooledObject.ReturnToPool();
```

### 2. Modifying Prefabs at Runtime

Don't modify the original prefabs at runtime:

```csharp
// DON'T DO THIS:
bulletPrefab.GetComponent<Bullet>().damage = 20;

// DO THIS INSTEAD:
GameObject bullet = _poolManager.GetPooledObject(bulletPrefab);
bullet.GetComponent<Bullet>().damage = 20;
```

### 3. Ignoring Pool Limits

Don't set unreasonably high or low pool limits:

```csharp
// DON'T DO THIS:
_poolManager.CreatePool(bulletPrefab, 1000, 10000); // Too many objects

// DO THIS INSTEAD:
_poolManager.CreatePool(bulletPrefab, 20, 100); // Reasonable limits
```

### 4. Forgetting to Reset Object State

Don't forget to reset the state of objects when retrieving them from the pool:

```csharp
// DON'T DO THIS:
GameObject enemy = _poolManager.GetPooledObject(enemyPrefab);
// Using the enemy without resetting its state

// DO THIS INSTEAD:
GameObject enemy = _poolManager.GetPooledObject(enemyPrefab);
enemy.GetComponent<Enemy>().ResetState();
```

### 5. Creating Unnecessary Pools

Don't create pools for objects that are rarely instantiated:

```csharp
// DON'T DO THIS:
// Creating pools for every possible object in the game

// DO THIS INSTEAD:
// Only create pools for frequently used objects
```

### 6. Ignoring Hierarchy Organization

Don't leave pooled objects in the scene hierarchy when returned to the pool:

```csharp
// DON'T DO THIS:
// Manually setting transform.parent = null and not resetting it when returning to pool

// The PoolManager handles this automatically
```

### 7. Excessive Component Lookups

Don't perform excessive GetComponent calls on pooled objects:

```csharp
// DON'T DO THIS:
void Update()
{
    // Getting component every frame
    gameObject.GetComponent<PooledObject>().ReturnToPool();
}

// DO THIS INSTEAD:
private PooledObject _pooledObject;

void Awake()
{
    _pooledObject = GetComponent<PooledObject>();
}

void ReturnObject()
{
    _pooledObject.ReturnToPool();
}
```

## Performance Considerations

### 1. Pool Size

Choose appropriate initial and maximum pool sizes based on your game's needs:

- **Initial Size**: How many objects you expect to need at the start
- **Maximum Size**: The upper limit to prevent memory issues

### 2. Component Caching

Cache component references to avoid GetComponent calls:

```csharp
private Rigidbody _rigidbody;
private PooledObject _pooledObject;

private void Awake()
{
    _rigidbody = GetComponent<Rigidbody>();
    _pooledObject = GetComponent<PooledObject>();
}
```

### 3. Batch Operations

When possible, batch pool operations:

```csharp
// Preload all pools at once during loading screens
// Return all objects at once when clearing levels
```

### 4. Monitor Pool Usage

Monitor your pool usage in development to optimize sizes:

```csharp
// In a debug build, you could log pool statistics
void LogPoolStats()
{
    Debug.Log($"Active bullets: {activeCount}, Total bullets: {totalCount}");
}
```
