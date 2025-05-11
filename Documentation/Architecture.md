# Ludo Pools Architecture

## Overview

Ludo Pools is designed as a lightweight, efficient object pooling system for Unity games. The architecture follows a simple, modular approach that makes it easy to integrate with existing projects while providing powerful pooling capabilities.

## Core Components

### PoolManager

The `PoolManager` is the central component of the system. It:

- Manages multiple object pools
- Handles object creation, retrieval, and return
- Provides a singleton instance for easy access
- Implements the `IPoolManager` interface

The PoolManager uses Unity's built-in `ObjectPool<T>` class internally to manage the actual pooling logic, providing a more user-friendly API on top of it.

### IPoolManager

The `IPoolManager` interface defines the contract for pool management. This allows for:

- Dependency injection in systems that need pooling functionality
- Easy mocking for testing
- Potential alternative implementations

### PooledObject

The `PooledObject` component is attached to all pooled objects. It:

- Maintains a reference to its original prefab
- Keeps a reference to the pool manager
- Provides a convenient way to return objects to the pool

## Helper Components

### ReturnToPoolAfterDelay

This component provides automatic return-to-pool functionality after a specified time delay. It's useful for:

- Particle effects that should be returned after playing
- Temporary objects that should disappear after a set time
- Any object with a predictable lifetime

### ReturnToPoolAction

This component provides a simple method that can be called to return an object to the pool. It's useful for:

- UI buttons that should trigger object return
- Animation events that should return objects
- Any scenario where you need to return an object via UnityEvents

## Extension Methods

### GameObjectExtensions

The `GameObjectExtensions` class provides extension methods for GameObject to simplify pool operations:

- `ReturnToPool()`: A convenient way to return any GameObject to its pool

## Object Lifecycle

1. **Pool Creation**:
   - Pools are created with an initial size and maximum size
   - Initial objects are instantiated and returned to the pool

2. **Object Retrieval**:
   - Objects are retrieved from the pool via `GetPooledObject()`
   - If the pool doesn't exist, it's created automatically
   - If the pool is empty, new objects are created (up to the maximum size)

3. **Object Usage**:
   - Objects are used in the game as normal GameObjects
   - They maintain their PooledObject component for tracking

4. **Object Return**:
   - Objects are returned to the pool via various methods:
     - Direct call to PoolManager
     - Through the PooledObject component
     - Via extension methods
     - Using helper components

5. **Pool Cleanup**:
   - Pools can be destroyed individually or all at once
   - All objects in the pool are properly destroyed

## Integration Points

The Ludo Pools system is designed to integrate easily with other systems:

- **Dependency Injection**: The IPoolManager interface can be injected into any system that needs pooling
- **Event Systems**: The ReturnToPoolAction component works with any event system
- **Animation**: Can be triggered via Animation Events
- **UI**: Can be triggered via UI events
- **Particle Systems**: Can be used with ParticleSystem.OnParticleSystemStopped
