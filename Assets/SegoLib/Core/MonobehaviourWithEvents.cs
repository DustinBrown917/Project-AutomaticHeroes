using UnityEngine;
using UnityEngine.Events;

namespace SegoLib
{
    public abstract class MonobehaviourWithEvents : MonoBehaviour
    {
        public class UnityEvent_Bool : UnityEvent<bool> { }

        public UnityEvent onAwake;
        public UnityEvent onEnable;
        public UnityEvent onStart;
        public UnityEvent onDisable;
        public UnityEvent onDestroy;

        protected virtual void Awake()
        {
            if (onAwake != null) {
                onAwake.Invoke();
            }
        }

        protected virtual void OnEnable()
        {
            if (onEnable != null) {
                onEnable.Invoke();
            }
        }

        protected virtual void Start()
        {
            if (onStart != null) {
                onStart.Invoke();
            }
        }

        protected virtual void OnDisable()
        {
            if (onDisable != null) {
                onDisable.Invoke();
            }
        }

        protected virtual void OnDestroy()
        {
            if (onDestroy != null) {
                onDestroy.Invoke();
            }
        }
    }
}
