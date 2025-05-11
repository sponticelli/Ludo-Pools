# Ludo Pools

A lightweight object pooling system for Unity games to efficiently manage and reuse game objects.

## Features

- Efficient object pooling to reduce garbage collection and improve performance
- Simple API for spawning and returning objects to the pool
- Automatic pool expansion when needed
- Helper components for common pooling operations
- Extension methods for GameObject pooling
- Dependency injection support via IPoolManager interface
- Comprehensive documentation and examples

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

## Quick Start

### Basic Usage

```csharp
// Using dependency injection (recommended)
[Inject] private IPoolManager _poolManager;

// Preload objects (optional)
_poolManager.CreatePool(prefab, 10, 100);

// Get an object from the pool
GameObject obj = _poolManager.GetPooledObject(prefab);
obj.transform.position = position;
obj.transform.rotation = rotation;

// Return an object to the pool
_poolManager.ReturnPooledObject(obj);

// Or use the extension method
obj.ReturnToPool();
```

### Using the ReturnToPoolAfterDelay Component

Add the `ReturnToPoolAfterDelay` component to automatically return an object to the pool after a specified time:

```csharp
// Get a reference to the component
var returnComponent = obj.GetComponent<ReturnToPoolAfterDelay>();

// Set the delay
returnComponent.delay = 3.0f;
```

### Using the ReturnToPoolAction Component

Add the `ReturnToPoolAction` component to return an object to the pool when a specific action occurs:

```csharp
// Get a reference to the component
var returnAction = obj.GetComponent<ReturnToPoolAction>();

// Call the Execute method to return the object to the pool
returnAction.Execute();

// Or assign it to a UnityEvent in the inspector
```

## Documentation

Comprehensive documentation is available in the Documentation folder:

- [API Documentation](Documentation/API-Documentation.md) - Detailed API reference
- [Architecture](Documentation/Architecture.md) - Overview of the system architecture
- [Usage Guide](Documentation/Usage-Guide.md) - In-depth usage instructions
- [Examples](Documentation/Examples.md) - Code examples for common scenarios
- [Patterns & Anti-Patterns](Documentation/Patterns-Antipatterns.md) - Best practices and things to avoid

## Integration

Ludo Pools is designed to integrate seamlessly with other systems:

- **Dependency Injection**: Use the IPoolManager interface for easy injection
- **Event Systems**: Works with any event system through the ReturnToPoolAction component
- **Animation**: Can be triggered via Animation Events
- **UI**: Can be triggered via UI events
- **Particle Systems**: Ideal for managing particle effects

## Performance Considerations

- Preload pools for frequently used objects to avoid runtime allocations
- Choose appropriate initial and maximum pool sizes
- Cache component references to avoid GetComponent calls
- Return objects to the pool instead of destroying them

## License

This project is licensed under the MIT License - see the LICENSE file for details.
