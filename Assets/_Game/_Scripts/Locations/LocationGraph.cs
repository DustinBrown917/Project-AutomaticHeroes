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
            public readonly ILocation location;

            public readonly List<Neighbour> neighbours = new();

            public Node(ILocation location)
            {
                this.location = location;
            }
        }

        /// <summary>
        /// Holds a neighbour location, and the route to get there. Only meaningful when relative to another LocationGraph.Node.
        /// </summary>
        public class Neighbour
        {
            public readonly ILocation neighbour;
            public readonly IRoute route;

            public Neighbour(ILocation neighbour, IRoute route)
            {
                this.neighbour = neighbour;
                this.route = route;
            }
        }

        /// <summary>
        /// All nodes in this graph, organized where the location Id is reliably the index of the location's node in this list.
        /// </summary>
        public IReadOnlyList<Node> Nodes => m_nodes;
        private readonly List<Node> m_nodes = new();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="locations"></param>
        public LocationGraph(IReadOnlyList<ILocation> locations)
        {
            for (int i = 0; i < locations.Count; i++) {
                m_nodes.Add(new Node(locations[i]));
            }

            foreach (Node node in m_nodes) {
                foreach (IRoute route in node.location.Routes) {
                    if (route.A == node.location) {
                        node.neighbours.Add(new Neighbour(route.B, route));
                    } else {
                        node.neighbours.Add(new Neighbour(route.A, route));
                    }
                }
            }
        }
    }
}
