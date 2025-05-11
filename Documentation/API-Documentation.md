# Ludo Pools API Documentation

## Core Classes

### PoolManager

The `PoolManager` is the central class that manages all object pools. It implements the `IPoolManager` interface.

#### Properties

- `Instance`: Static singleton instance of the PoolManager.

#### Methods

- `CreatePool(GameObject prefab, int initialPoolSize, int maxPoolSize)`: Creates a new pool for the specified prefab.
- `GetPooledObject(GameObject prefab)`: Gets an object from the pool. If the pool doesn't exist, it creates one automatically.
- `ReturnPooledObject(GameObject prefab, GameObject obj)`: Returns an object to its pool.
- `ReturnPooledObject(GameObject obj)`: Returns an object to its pool using the PooledObject component.
- `ReturnAllPooledObjects()`: Returns all active objects to their respective pools.
- `CleanupPools()`: Destroys all pools and their objects.
- `DestroyPool(GameObject prefab)`: Destroys a specific pool and all its objects.

#### Example

```csharp
// Get the pool manager
var poolManager = PoolManager.Instance;

// Create a pool with 10 initial objects and a maximum of 100
poolManager.CreatePool(myPrefab, 10, 100);

// Get an object from the pool
GameObject obj = poolManager.GetPooledObject(myPrefab);

// Return the object to the pool
poolManager.ReturnPooledObject(myPrefab, obj);
```

### IPoolManager

Interface that defines the core functionality for object pooling.

#### Methods

- `GameObject GetPooledObject(GameObject prefab)`: Gets an object from the pool.
- `void ReturnPooledObject(GameObject prefab, GameObject obj)`: Returns an object to its pool.
- `void ReturnPooledObject(GameObject obj)`: Returns an object to its pool using the PooledObject component.
- `void ReturnAllPooledObjects()`: Returns all active objects to their respective pools.
- `void CleanupPools()`: Destroys all pools and their objects.
- `void CreatePool(GameObject prefab, int initialPoolSize, int maxPoolSize)`: Creates a new pool.
- `void DestroyPool(GameObject prefab)`: Destroys a specific pool.

### PooledObject

Component attached to pooled objects to track their state and provide easy access to pool functionality.

#### Methods

- `SetPool(IPoolManager manager, GameObject original)`: Sets the pool manager and prefab reference.
- `ReturnToPool()`: Returns the object to its pool.

#### Example

```csharp
// Get the PooledObject component
var pooledObj = obj.GetComponent<PooledObject>();

// Return the object to its pool
pooledObj.ReturnToPool();
```

## Helper Components

### ReturnToPoolAfterDelay

Component that automatically returns an object to the pool after a specified delay.

#### Properties

- `delay`: The delay in seconds before returning the object to the pool.

#### Methods

- `StartTimer()`: Starts the timer for returning the object.
- `CancelTimer()`: Cancels the timer.

#### Example

```csharp
// Get the ReturnToPoolAfterDelay component
var returnComponent = obj.GetComponent<ReturnToPoolAfterDelay>();

// Set the delay
returnComponent.delay = 3.0f;

// Start the timer
returnComponent.StartTimer();
```

### ReturnToPoolAction

Component that provides a method to return an object to the pool when triggered by an event or action.

#### Methods

- `Execute()`: Returns the object to the pool.

#### Example

```csharp
// Get the ReturnToPoolAction component
var returnAction = obj.GetComponent<ReturnToPoolAction>();

// Trigger the return action
returnAction.Execute();
```

## Extension Methods

### GameObjectExtensions

Provides extension methods for GameObject to simplify pool operations.

#### Methods

- `ReturnToPool(this GameObject gameObject)`: Returns the GameObject to its pool.

#### Example

```csharp
// Return the object to its pool using the extension method
obj.ReturnToPool();
```
