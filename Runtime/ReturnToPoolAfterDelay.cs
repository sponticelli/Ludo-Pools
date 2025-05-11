using UnityEngine;
using UnityEngine.Serialization;

namespace Ludo.Core.Pools.Runtime
{
    public class ReturnToPoolAfterDelay : MonoBehaviour
    {
        [SerializeField] private float delay = 1f;
        
        private float _timer;
        private bool _triggered;
        
        private void OnEnable()
        {
            _timer = 0f;
            _triggered = false;
        }
        
        private void Update()
        {
            if (_triggered) return;
            
            _timer += Time.deltaTime;
            if (!(_timer >= delay)) return;
            
            _triggered = true;
            gameObject.ReturnToPool();
        }
    }

}