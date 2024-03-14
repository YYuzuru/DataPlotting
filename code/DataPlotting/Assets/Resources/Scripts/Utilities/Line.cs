using UnityEngine;

namespace GraphPlotter.Utilities
{
    public class Line
    {
        public Vector2 direction;

        public Vector2 end;

        public Vector2 normal;

        public Vector2 start;

        public Line(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
            direction = end - start;
            normal = new Vector2(-direction.y, direction.x).normalized;
        }

        public void Refresh()
        {
            direction = end - start;
            normal = new Vector2(-direction.y, direction.x).normalized;
        }
    }
}