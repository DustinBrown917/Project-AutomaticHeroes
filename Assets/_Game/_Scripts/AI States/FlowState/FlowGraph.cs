using XNode;

namespace Game.Flow
{
    public class FlowGraph : NodeGraph
    {
        [field: System.NonSerialized] public bool Initialized { get; private set; }

        private void OnEnable()
        {
            Initialize();
        }
        private void OnDisable()
        {
            OnDisposed();
        }

        public void Initialize()
        {
            if(Initialized) 
                return;

            OnInitialize();
        }


        protected virtual void OnInitialize() { }
        protected virtual void OnDisposed() { }
    } 
}
