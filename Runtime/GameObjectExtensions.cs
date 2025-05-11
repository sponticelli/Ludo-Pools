using UnityEngine;

namespace Ludo.Core.Pools.Runtime
{
    public static class GameObjectExtensions
    {
        public static void ReturnToPool(this GameObject gameObject)
        {
            var pooledObject = gameObject.GetComponent<PooledObject>();
            if (pooledObject != null)
            {
                pooledObject.ReturnToPool();
            }
            else
            {
                Debug.LogWarning("PooledObject component is missing. Object will be destroyed.");
                Object.Destroy(gameObject);
            }
        }
    }
}