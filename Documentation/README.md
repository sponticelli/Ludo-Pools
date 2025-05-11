# Ludo Pools Documentation

## Overview

Ludo Pools is a lightweight object pooling system for Unity games. It provides an efficient way to manage and reuse game objects, reducing garbage collection and improving performance.

## API Reference

### PoolManager

The main class responsible for managing object pools.

#### Methods

- `Preload(GameObject prefab, int count)`: Preloads a specified number of instances of a prefab.
- `Spawn(GameObject prefab, Vector3 position, Quaternion rotation)`: Spawns an object from the pool.
- `Return(GameObject obj)`: Returns an object to the pool.
- `Clear()`: Clears all pools.

### PooledObject

A component attached to pooled objects to track their state.

#### Properties

- `Prefab`: The prefab this object was instantiated from.
- `IsPooled`: Whether the object is currently in the pool.

### ReturnToPoolAfterDelay

A component that automatically returns an object to the pool after a specified delay.

#### Methods

- `StartTimer()`: Starts the timer for returning the object.
- `CancelTimer()`: Cancels the timer.

### ReturnToPoolAction

A component that returns an object to the pool when a specific action occurs.

## Usage Examples

See the main README.md file for usage examples.
