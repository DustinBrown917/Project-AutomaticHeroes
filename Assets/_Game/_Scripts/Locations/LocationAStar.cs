using SegoLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Searches a <see cref="LocationGraph"/> for the shortest route-based path between locations.
    /// <para>
    /// <see cref="TryFindPath"/> is A* towards a known destination. <see cref="TryFindNearest"/> is the same search with a zero
    /// heuristic (Dijkstra), for when the destination is "whichever matching location is closest by road".
    /// </para>
    /// </summary>
    public static class LocationAStar
    {
        private enum EState : byte
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
            public EState state;
            /// <summary>
            /// Cost of path so far.
            /// </summary>
            public float gCost = float.MaxValue;
            /// <summary>
            /// Heuristic cost to goal.
            /// </summary>
            public float hCost;
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

        /// <summary>
        /// Straight line distance. A route always runs from one location to the other, so it can never be shorter than this,
        /// which keeps the heuristic admissible and consistent.
        /// </summary>
        public static float GetHeuristic(ILocation from, ILocation to)
        {
            return Vector2.Distance(from.Position, to.Position);
        }

        /// <summary>
        /// Find the shortest path from <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        /// <param name="path">Optional. Cleared, then filled with the traversals to take in order. Empty if <paramref name="start"/> is <paramref name="end"/>.</param>
        /// <returns>False if either location isn't in <paramref name="graph"/>, or <paramref name="end"/> can't be reached.</returns>
        public static bool TryFindPath(LocationGraph graph, ILocation start, ILocation end, List<IRouteTraversal> path = null)
        {
            if (end == null) {
                path?.Clear();
                return false;
            }

            return TrySearch(graph, start, location => location == end, location => GetHeuristic(location, end), out _, path);
        }

        /// <summary>
        /// Find the location closest to <paramref name="start"/> by road that satisfies <paramref name="isGoal"/>. <paramref name="start"/> itself counts.
        /// </summary>
        /// <param name="nearest">The closest matching location, or null if none can be reached.</param>
        /// <param name="path">Optional. Cleared, then filled with the traversals to take in order.</param>
        public static bool TryFindNearest(LocationGraph graph, ILocation start, Predicate<ILocation> isGoal, out ILocation nearest, List<IRouteTraversal> path = null)
        {
            return TrySearch(graph, start, isGoal, _ => 0f, out nearest, path);
        }

        private static bool TrySearch(LocationGraph graph, ILocation start, Predicate<ILocation> isGoal, Func<ILocation, float> heuristic, out ILocation goal, List<IRouteTraversal> path)
        {
            path?.Clear();
            goal = null;

            if (!graph.TryGetNode(start, out LocationGraph.Node startNode))
                return false;

            NavData[] navData = new NavData[graph.Nodes.Count];

            for (int i = 0; i < graph.Nodes.Count; i++) {
                navData[i] = new() { node = graph.Nodes[i] };
            }

            NavData startData = navData[startNode.index];
            startData.gCost = 0;
            startData.hCost = heuristic(start);
            startData.state = EState.Open;

            List<NavData> openSet = new() { startData };

            while (openSet.Count > 0) {
                NavData currentNode = openSet[0];
                openSet.RemoveAt(0);

                currentNode.state = EState.Closed;

                if (isGoal(currentNode.node.location)) {
                    goal = currentNode.node.location;

                    if (path != null)
                        RetracePath(currentNode, path);

                    return true;
                }

                foreach (LocationGraph.Neighbour locationNeighbour in currentNode.node.neighbours) {
                    NavData neighbourData = navData[locationNeighbour.neighbour.index];

                    if (neighbourData.state == EState.Closed)
                        continue;

                    float newGCost = currentNode.gCost + locationNeighbour.route.Length;

                    if (newGCost >= neighbourData.gCost)
                        continue;

                    // The open set is sorted, so take the node out before its cost changes and put it back after.
                    if (neighbourData.state == EState.Open) {
                        openSet.Remove(neighbourData);
                    } else {
                        neighbourData.hCost = heuristic(neighbourData.node.location);
                        neighbourData.state = EState.Open;
                    }

                    neighbourData.gCost = newGCost;
                    neighbourData.comingFrom = currentNode;
                    neighbourData.routeUsed = locationNeighbour.route;

                    openSet.OrderedInsert(neighbourData);
                }
            }

            return false;
        }

        private static void RetracePath(NavData end, List<IRouteTraversal> path)
        {
            NavData currentNode = end;

            while (currentNode.comingFrom != null) {

                if (currentNode.node.location == currentNode.routeUsed.A) {
                    path.Add(new RouteTraversalBToA(currentNode.routeUsed));
                } else {
                    path.Add(new RouteTraversalAToB(currentNode.routeUsed));
                }

                currentNode = currentNode.comingFrom;
            }

            path.Reverse();
        }
    }
}
