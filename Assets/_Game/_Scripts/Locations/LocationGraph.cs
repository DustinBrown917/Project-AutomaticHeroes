using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Data structure used for organizing locations by associating them with their neighbours, and the routes connecting them to those neighbours.
    /// </summary>
    public class LocationGraph
    {
        /// <summary>
        /// Holds a location and all its neighbours.
        /// </summary>
        public class Node
        {
            public readonly int index;

            public readonly ILocation location;

            public readonly List<Neighbour> neighbours = new();

            public Node(int index, ILocation location)
            {
                this.index = index;
                this.location = location;
            }
        }

        /// <summary>
        /// Holds a neighbour location, and the route to get there. Only meaningful when relative to another LocationGraph.Node.
        /// </summary>
        public class Neighbour
        {
            public readonly Node neighbour;
            public readonly IRoute route;

            public Neighbour(Node neighbour, IRoute route)
            {
                this.neighbour = neighbour;
                this.route = route;
            }
        }

        /// <summary>
        /// All nodes in this graph. A node's <see cref="Node.index"/> is its index in this list.
        /// </summary>
        public IReadOnlyList<Node> Nodes => m_nodes;
        private readonly List<Node> m_nodes = new();

        private readonly Dictionary<ILocation, Node> m_nodesByLocation = new();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="locations"></param>
        public LocationGraph(IReadOnlyList<ILocation> locations)
        {
            for (int i = 0; i < locations.Count; i++) {
                Node node = new Node(i, locations[i]);
                m_nodes.Add(node);
                m_nodesByLocation.Add(locations[i], node);
            }

            foreach (Node node in m_nodes) {
                foreach (IRoute route in node.location.Routes) {
                    ILocation other = route.A == node.location ? route.B : route.A;

                    // Routes to locations outside this graph are ignored rather than half-linked.
                    if (m_nodesByLocation.TryGetValue(other, out Node otherNode)) {
                        node.neighbours.Add(new Neighbour(otherNode, route));
                    }
                }
            }
        }

        /// <summary>
        /// Get the node for <paramref name="location"/>, if <paramref name="location"/> is part of this graph.
        /// </summary>
        public bool TryGetNode(ILocation location, out Node node)
        {
            if (location == null) {
                node = null;
                return false;
            }

            return m_nodesByLocation.TryGetValue(location, out node);
        }
    }
}
