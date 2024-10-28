
namespace Game.Monsters.Combat
{
    [System.Serializable]
    public class PortData
    {
        public static void ConnectPortDatas(PortData a, PortData b)
        {
            if(a != null) {
                a.SetConnectedPort(b);
            }

            if(b != null) {
                b.SetConnectedPort(a);
            }
        }

        [field: System.NonSerialized] public Node Node { get; private set; }
        [field: System.NonSerialized] public PortData ConnectedPort { get; private set; }

        [field: System.NonSerialized] public bool Initialized { get; private set; }

        private void SetConnectedPort(PortData other)
        {
            ConnectedPort = other;
        }

        public void Initialize(Node node)
        {
            if (Initialized)
                return;

            Node = node;
            Initialized = true;
        }
    } 
}
