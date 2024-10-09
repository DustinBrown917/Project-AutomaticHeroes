using UnityEngine;

namespace Game
{

    public interface IRouteTraversal
    {
        public IRoute Route { get; }

        public ILocation Start { get; }
        public ILocation End { get; }

        public float Length { get; }

        public Vector2 Traverse(float t);

        public IRouteTraversal Reverse();
    }

    public class RouteTraversalAToB : IRouteTraversal
    {
        public IRoute Route { get; }

        public ILocation Start => Route.A;

        public ILocation End => Route.B;

        public float Length => Route.Length;

        public RouteTraversalAToB(IRoute route)
        {
            Route = route;
        }

        public Vector2 Traverse(float t)
        {
            return Route.TraverseAToB(t);
        }

        public IRouteTraversal Reverse()
        {
            return new RouteTraversalBToA(Route);
        }
    }

    public class RouteTraversalBToA : IRouteTraversal
    {
        public IRoute Route { get; }

        public ILocation Start => Route.B;

        public ILocation End => Route.A;

        public float Length => Route.Length;

        public RouteTraversalBToA(IRoute route)
        {
            Route = route;
        }

        public Vector2 Traverse(float t)
        {
            return Route.TraverseBToA(t);
        }

        public IRouteTraversal Reverse()
        {
            return new RouteTraversalAToB(Route);
        }
    }
}
