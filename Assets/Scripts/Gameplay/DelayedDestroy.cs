using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class DelayedDestroy : MonoBehaviour
    {
        [SerializeField] float delay;
        private Coroutine destroyCoroutine;

        [SerializeField] private UnityEvent OnDestroy;
    
        public void SetToBeDestroyed()
        {
            if (destroyCoroutine == null)
            {
                destroyCoroutine = StartCoroutine(DestroyRoutine());
            }
        }
    
        private IEnumerator DestroyRoutine()
        {
            yield return new WaitForSeconds(delay);
            
            OnDestroy?.Invoke();
            
            Destroy(gameObject);
        }
    }
}