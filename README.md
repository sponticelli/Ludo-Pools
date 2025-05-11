# Ludo Pools

A lightweight object pooling system for Unity games to efficiently manage and reuse game objects.

## Features

- Efficient object pooling to reduce garbage collection and improve performance
- Simple API for spawning and returning objects to the pool
- Automatic pool expansion when needed
- Helper components for common pooling operations
- Extension methods for GameObject pooling

## Installation

### Using Unity Package Manager (UPM)

1. Open the Package Manager window in Unity (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter the following URL: `https://github.com/sponticelli/Ludo-Pools.git`
5. Click "Add"

### Manual Installation

1. Clone or download this repository
2. Copy the contents to your Unity project's Assets folder

## Usage

### Basic Usage

```csharp
// Get the pool manager
var poolManager = PoolManager.Instance;

// Preload objects (optional)
poolManager.Preload(prefab, 10);

// Spawn an object from the pool
GameObject obj = poolManager.Spawn(prefab, position, rotation);

// Return an object to the pool
poolManager.Return(obj);

// Or use the extension method
obj.ReturnToPool();
```

### Using the ReturnToPoolAfterDelay Component

Add the `ReturnToPoolAfterDelay` component to automatically return an object to the pool after a specified time:

```csharp
// Get a reference to the component
var returnComponent = obj.GetComponent<ReturnToPoolAfterDelay>();

// Set the delay
returnComponent.Delay = 3.0f;

// Start the timer
returnComponent.StartTimer();
```

### Using the ReturnToPoolAction Component

Add the `ReturnToPoolAction` component to return an object to the pool when a specific action occurs.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
