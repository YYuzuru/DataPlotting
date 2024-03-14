using UnityEngine;

namespace GraphPlotter.Utilities
{
    public static class PlanarIntersection
    {
        //https://www.habrador.com/tutorials/math/5-line-line-intersection/
        public static bool IntersectsLine(Line A, Line B, ref Vector2 intersection)
        {
            Vector2 src = intersection;
            if (IsOrthogonal(A.start - B.start, A.normal) || IsParallel(A.normal, B.normal))
                return false;

            float k1 = (A.normal.x * A.start.x) + (A.normal.y * A.start.y);
            float k2 = (B.normal.x * B.start.x) + (B.normal.y * B.start.y);

            float det = Determinant(A.normal, B.normal);
            intersection.x = (B.normal.y * k1 - A.normal.y * k2) / det;
            intersection.y = (-B.normal.x * k1 + A.normal.x * k2) / det;

            if (IsBetweenLine(A, intersection) && IsBetweenLine(B, intersection))
                return true;
            intersection = src;
            return false;
        }
        public static bool Intersects(Line A, Line B, ref Vector2 intersection)
        {
            if (IsOrthogonal(A.start - B.start, A.normal) || IsParallel(A.normal, B.normal))
                return false;

            float k1 = (A.normal.x * A.start.x) + (A.normal.y * A.start.y);
            float k2 = (B.normal.x * B.start.x) + (B.normal.y * B.start.y);

            float det = Determinant(A.normal, B.normal);
            intersection.x = (B.normal.y * k1 - A.normal.y * k2) / det;
            intersection.y = (-B.normal.x * k1 + A.normal.x * k2) / det;

            return true;
        }

        private static float Determinant(Vector2 a, Vector2 b)
        {
            return (a.x * b.y - a.y * b.x);
        }

        private static bool IsBetweenLine(Line A, Vector2 point)
        {
            Vector2 ac = point - A.start;

            //Check Direction and length
            return (Vector2.Dot(A.direction, ac) > 0f && A.direction.sqrMagnitude >= ac.sqrMagnitude);
        }

        private static bool IsOrthogonal(Vector2 v1, Vector2 v2)
        {
            return (Mathf.Abs(Vector2.Dot(v1, v2)) < 0.000001f);
        }

        private static bool IsParallel(Vector2 v1, Vector2 v2)
        {
            return (Vector2.Angle(v1, v2) == 0f || Vector2.Angle(v1, v2) == 180f);
        }
    }
}