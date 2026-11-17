using SegoLib;
using XNode;

namespace Game.Flow
{
    public class FlowGraph : NodeGraph, IInitializable
    {
        [field: System.NonSerialized] public bool IsInitialized { get; private set; }

        private void OnEnable()
        {
            Initialize();
        }
        private void OnDisable()
        {
            OnDisposed();
            IsInitialized = false;
        }

        public void Initialize()
        {
            if(IsInitialized) 
                return;

            OnInitialize();

            IsInitialized = true;
        }


        protected virtual void OnInitialize() { }
        protected virtual void OnDisposed() { }
    } 
}
