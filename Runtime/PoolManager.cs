using System.Collections.Generic;
using Ludo.UnityInject;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace Ludo.Core.Pools.Runtime
{
    public class PoolManager : MonoBehaviour, IPoolManager
    {
        [System.Serializable]
        public class PooledObjectInfo
        {
            public GameObject prefab;
            public int initialPoolSize;
            public int maxPoolSize;
            [HideInInspector] public Transform poolParent;
        }

        public List<PooledObjectInfo> pooledObjects;

        private Dictionary<GameObject, ObjectPool<GameObject>> pools;
        private Dictionary<GameObject, List<GameObject>> activeObjects;
        private bool _isApplicationQuitting;
        
        private IContainer _container;

        public bool IsInitialized { get; private set; } = false;
        
        
        [Inject]
        [Preserve]
        public void InitializeWithDependencies(IContainer container)
        {
            if (IsInitialized) return;
            // Debug.Log($"PoolManager: Container injected: {container != null}");
            _container = container;
            InitializePools(); 
        }
        

        private void InitializePools()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
            activeObjects = new Dictionary<GameObject, List<GameObject>>();

            foreach (var obj in pooledObjects)
            {
                CreatePool(obj);
            }
        }

        private void CreatePool(PooledObjectInfo obj)
        {
            obj.poolParent = new GameObject(obj.prefab.name + "Pool").transform;
            obj.poolParent.SetParent(transform);

            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject newObj = _container.InstantiatePrefab(obj.prefab);
                    newObj.transform.SetParent(obj.poolParent);
                    newObj.name = obj.prefab.name + " " + obj.poolParent.childCount;
                    PooledObject pooledObj = newObj.AddComponent<PooledObject>();
                    pooledObj.SetPool(this, obj.prefab);
                    newObj.SetActive(false);
                    return newObj;
                },
                actionOnGet: (GameObject pooledObject) =>
                {
                    pooledObject.SetActive(true);
                    if (!activeObjects.ContainsKey(obj.prefab))
                    {
                        activeObjects[obj.prefab] = new List<GameObject>();
                    }

                    activeObjects[obj.prefab].Add(pooledObject);
                },
                actionOnRelease: (GameObject pooledObject) =>
                {
                    if (pooledObject == null) return;
                    pooledObject.SetActive(false);
                    pooledObject.transform.SetParent(obj.poolParent);
                    if (activeObjects.ContainsKey(obj.prefab))
                    {
                        activeObjects[obj.prefab].Remove(pooledObject);
                    }
                },
                actionOnDestroy: (GameObject pooledObject) => Destroy(pooledObject),
                collectionCheck: false,
                defaultCapacity: obj.initialPoolSize,
                maxSize: obj.maxPoolSize
            );

            pools.Add(obj.prefab, pool);
                
                
            GameObject[] items = new GameObject[obj.initialPoolSize];
            for (int i = 0; i < obj.initialPoolSize; i++)
            {
                items[i] = pool.Get();
            }

            for (int i = 0; i < obj.initialPoolSize; i++)
            {
                pool.Release(items[i]);
            }
        }

        public GameObject GetPooledObject(GameObject prefab)
        {
            if (!pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                // Auto-create a pool with default settings
                CreatePool(prefab, 10, 100);
                pool = pools[prefab];
            }

            GameObject obj = pool.Get();
            obj.transform.SetParent(null);
            return obj;
        }

        public void ReturnPooledObject(GameObject prefab, GameObject obj)
        {
            if (pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogWarning($"Pool for prefab {prefab.name} doesn't exist. Object will be destroyed.");
                Destroy(obj);
            }
        }
        
        public void ReturnPooledObject(GameObject obj)
        {
            PooledObject pooledObj = obj.GetComponent<PooledObject>();
            if (pooledObj != null)
            {
                pooledObj.ReturnToPool();
            }
            else
            {
                Debug.LogWarning("Cannot return object to pool. Missing PooledObject component or prefab reference.");
                Destroy(obj);
            }
        }

        public void ReturnAllPooledObjects()
        {
            foreach (var kvp in activeObjects)
            {
                GameObject prefab = kvp.Key;
                List<GameObject>
                    objects = new List<GameObject>(kvp.Value); // Create a copy to avoid modification during iteration
                foreach (GameObject obj in objects)
                {
                    ReturnPooledObject(prefab, obj);
                }
            }
        }

        public void CleanupPools()
        {
            foreach (var obj in pooledObjects)
            {
                if (obj.poolParent != null)
                {
                    Destroy(obj.poolParent.gameObject);
                }
            }
            
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }

            pools.Clear();
            activeObjects.Clear();
        }

        public void CreatePool(GameObject prefab, int initialPoolSize, int maxPoolSize)
        {
            // Check if pool already exists
            if (pools.ContainsKey(prefab))
            {
                Debug.LogWarning($"Pool for prefab {prefab.name} already exists.");
                return;
            }
            
            
            PooledObjectInfo obj = new PooledObjectInfo
            {
                prefab = prefab,
                initialPoolSize = initialPoolSize,
                maxPoolSize = maxPoolSize
            };
            pooledObjects.Add(obj);
            CreatePool(obj);
        }

        public void DestroyPool(GameObject prefab)
        {
            if (_isApplicationQuitting)
            {
                return;
            }
            // Check if pool exists
            if (!pools.ContainsKey(prefab))
            {
                Debug.LogWarning($"Pool for prefab {prefab.name} doesn't exist.");
                return;
            }
            
            // Return all active objects to the pool
            if (activeObjects.TryGetValue(prefab, out List<GameObject> objects))
            {
                foreach (GameObject obj in objects)
                {
                    ReturnPooledObject(prefab, obj);
                }
            }
            
            // Destroy pool parent
            if (pooledObjects.Find(x => x.prefab == prefab) is { } poi)
            {
                Destroy(poi.poolParent.gameObject);
                pooledObjects.Remove(poi);
            }
        }

        private void OnApplicationQuit()
        {
            _isApplicationQuitting = true;
        }
    }
}