using UnityEngine;

namespace Ludo.Core.Pools.Runtime
{
    public class PooledObject : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool debug = false;
#endif
        
        private IPoolManager _poolManager;
        private GameObject _prefab;

        public void SetPool(IPoolManager manager, GameObject original)
        {
            Debug.Assert(manager != null, "PoolManager reference is missing.");
            _poolManager = manager;
            _prefab = original;
        }

        public void ReturnToPool()
        {
            if (_poolManager != null)
            {
                _poolManager.ReturnPooledObject(_prefab, gameObject);
            }
            else
            {
#if UNITY_EDITOR
                if (debug) Debug.LogWarning("PoolManager reference is missing. Object will be destroyed.");
#endif
                Destroy(gameObject);
            }
        }
    }
}