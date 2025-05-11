# Ludo Pools Usage Guide

## Getting Started

### Installation

1. **Using Unity Package Manager (UPM)**:
   - Open the Package Manager window in Unity (Window > Package Manager)
   - Click the "+" button in the top-left corner
   - Select "Add package from git URL..."
   - Enter the following URL: `https://github.com/sponticelli/Ludo-Pools.git`
   - Click "Add"

2. **Manual Installation**:
   - Clone or download the repository
   - Copy the contents to your Unity project's Assets folder

### Basic Setup

The PoolManager is designed to work with minimal setup. It automatically creates a singleton instance when first accessed:

```csharp
// Get the pool manager
var poolManager = PoolManager.Instance;
```

For dependency injection scenarios, you can reference the IPoolManager interface:

```csharp
[Inject] private IPoolManager _poolManager;
```

## Creating Pools

### Manual Pool Creation

You can create pools in advance to pre-instantiate objects:

```csharp
// Create a pool with 10 initial objects and a maximum of 100
poolManager.CreatePool(myPrefab, 10, 100);
```

### Automatic Pool Creation

Pools are created automatically when you request an object that doesn't have a pool yet:

```csharp
// This will create a pool if one doesn't exist
GameObject obj = poolManager.GetPooledObject(myPrefab);
```

## Using Pooled Objects

### Getting Objects from the Pool

```csharp
// Get an object from the pool
GameObject obj = poolManager.GetPooledObject(myPrefab);

// Position and rotate as needed
obj.transform.position = position;
obj.transform.rotation = rotation;
```

### Returning Objects to the Pool

There are several ways to return objects to the pool:

1. **Using the PoolManager directly**:
   ```csharp
   poolManager.ReturnPooledObject(prefab, obj);
   ```

2. **Using the PooledObject component**:
   ```csharp
   var pooledObj = obj.GetComponent<PooledObject>();
   pooledObj.ReturnToPool();
   ```

3. **Using the GameObject extension method**:
   ```csharp
   obj.ReturnToPool();
   ```

## Helper Components

### ReturnToPoolAfterDelay

This component automatically returns an object to the pool after a specified delay:

1. **Add the component to your prefab**:
   ```csharp
   var returnComponent = prefab.AddComponent<ReturnToPoolAfterDelay>();
   returnComponent.delay = 3.0f; // Set default delay
   ```

2. **Adjust the delay at runtime**:
   ```csharp
   var returnComponent = obj.GetComponent<ReturnToPoolAfterDelay>();
   returnComponent.delay = 5.0f; // Override the delay
   ```

### ReturnToPoolAction

This component provides a method that can be called to return an object to the pool:

1. **Add the component to your prefab**:
   ```csharp
   prefab.AddComponent<ReturnToPoolAction>();
   ```

2. **Call the Execute method**:
   ```csharp
   var returnAction = obj.GetComponent<ReturnToPoolAction>();
   returnAction.Execute();
   ```

3. **Use with UnityEvents**:
   ```csharp
   // In the Inspector, assign the ReturnToPoolAction.Execute method to a button click event
   button.onClick.AddListener(returnAction.Execute);
   ```

## Real-World Examples

### Spawning Enemies

```csharp
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [Inject] private IPoolManager _poolManager;

    public void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = _poolManager.GetPooledObject(enemyPrefab);
        enemy.transform.position = position;
        enemy.transform.rotation = Quaternion.identity;
        
        // Initialize enemy state if needed
        var enemyComponent = enemy.GetComponent<Enemy>();
        enemyComponent.Initialize();
    }
}
```

### Particle Effects

```csharp
public class EffectsManager : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [Inject] private IPoolManager _poolManager;

    public void PlayExplosion(Vector3 position)
    {
        GameObject explosion = _poolManager.GetPooledObject(explosionPrefab);
        explosion.transform.position = position;
        
        // The explosion prefab should have a ReturnToPoolAfterDelay component
        // that will automatically return it to the pool after the effect finishes
    }
}
```

### Projectiles

```csharp
public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [Inject] private IPoolManager _poolManager;

    public void Fire()
    {
        GameObject bullet = _poolManager.GetPooledObject(bulletPrefab);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        
        // Add velocity to the bullet
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }
    }
}
```
