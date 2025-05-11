using UnityEngine;

namespace Ludo.Core.Pools.Runtime
{
    public class ReturnToPoolAction : MonoBehaviour
    {
        public void Execute()
        {
            gameObject.ReturnToPool();
        }
    }
}