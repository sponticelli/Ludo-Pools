using UnityEngine;

namespace Ludo.Core.Pools.Runtime
{
    public interface IPoolManager 
    {
        GameObject GetPooledObject(GameObject prefab);
        void ReturnPooledObject(GameObject prefab, GameObject obj);
        void ReturnPooledObject(GameObject obj);
        void ReturnAllPooledObjects();
        void CleanupPools();
        
        void CreatePool(GameObject prefab, int initialPoolSize, int maxPoolSize);
        void DestroyPool(GameObject prefab);
    }
}