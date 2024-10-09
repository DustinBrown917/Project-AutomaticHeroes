using SegoLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Uses AStar to traverse a LocationGraph and find the best path from a starting location to an end location using routes.
    /// </summary>
    public static class LocationAStar
    {
        public enum State : byte
        {
            Unexplored = 0,
            Open = 1,
            Closed = 2
        }

        private class NavData : IComparable<NavData>
        {
            /// <summary>
            /// The node this data is for.
            /// </summary>
            public LocationGraph.Node node;
            /// <summary>
            /// State of this node.
            /// </summary>
            public State state;
            /// <summary>
            /// Cost of path so far.
            /// </summary>
            public float gCost = float.MaxValue;
            /// <summary>
            /// Heuristic cost to goal.
            /// </summary>
            public float hCost = -1;
            /// <summary>
            /// hCost + gCost.
            /// </summary>
            public float FCost => gCost + hCost;
            /// <summary>
            /// The node preceding this one in the path.
            /// </summary>
            public NavData comingFrom;
            /// <summary>
            /// Route used to get here.
            /// </summary>
            public IRoute routeUsed;

            public int CompareTo(NavData other)
            {
                return FCost.CompareTo(other.FCost);
            }
        }

        public static float GetHeuristic(ILocation from, ILocation to)
        {
            return Vector2.Distance(from.Position, to.Position);
        }

        public static List<IRouteTraversal> FindPath(LocationGraph graph, ILocation start, ILocation end)
        {
            if (start == end)
                return new List<IRouteTraversal>();

            NavData[] navData = new NavData[graph.Nodes.Count];

            for (int i = 0; i < graph.Nodes.Count; i++) {
                navData[i] = new() { node = graph.Nodes[i] };
            }

            navData[start.Id].gCost = 0;
            navData[end.Id].hCost = 0;

            List<NavData> openSet = new() { navData[start.Id] };

            while (openSet.Count > 0) {
                NavData currentNode = openSet[0];
                openSet.RemoveAt(0);

                currentNode.state = State.Closed;

                if (currentNode.node.location == end) {
                    return RetracePath(navData[start.Id], navData[end.Id]);
                }

                foreach (LocationGraph.Neighbour locationNeighbour in currentNode.node.neighbours) {
                    NavData neighbourData = navData[locationNeighbour.neighbour.Id];

                    if (neighbourData.state == State.Closed)
                        continue;

                    if (neighbourData.hCost < 0)
                        neighbourData.hCost = GetHeuristic(neighbourData.node.location, end);

                    float newGCost = locationNeighbour.route.Length + currentNode.gCost;

                    if (newGCost < neighbourData.gCost) {
                        neighbourData.gCost = newGCost;
                        neighbourData.comingFrom = currentNode;
                        neighbourData.routeUsed = locationNeighbour.route;

                        if (neighbourData.state == State.Open) {
                            openSet.Remove(neighbourData);
                            openSet.OrderedInsert(neighbourData);
                        }
                    }

                    if (neighbourData.state != State.Open) {
                        openSet.OrderedInsert(neighbourData);
                        neighbourData.state = State.Open;
                    }
                }
            }

            return new List<IRouteTraversal>();
        }

        private static List<IRouteTraversal> RetracePath(NavData start, NavData end)
        {
            List<IRouteTraversal> path = new();
            NavData currentNode = end;

            while (currentNode != start) {

                if(currentNode.node.location == currentNode.routeUsed.A) {
                    path.Add(new RouteTraversalBToA(currentNode.routeUsed));
                } else {
                    path.Add(new RouteTraversalAToB(currentNode.routeUsed));
                }

                currentNode = currentNode.comingFrom;
            }

            path.Reverse();
            return path;
        }
    }
}