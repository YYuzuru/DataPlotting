using GraphPlotter.Interface;
using GraphPlotter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace GraphPlotter
{
    public class LineGenerator : MaskableGraphic
    {
        private Color lineColor = Color.red;
        private Vector2[] dataPositions = new Vector2[0];
        public float lineStrength = 1;
        public ePlot2D type = ePlot2D.Line;

        private Vector2 scale = Vector2.one;
        private float xLeftBoundary = 0f;
        private float xRightBoundary = 0f;
        private float yLeftBoundary = 0f;
        private float yRightBoundary = 0f;
        private Vector2 zeroPosition = Vector2.zero;
        List<Vector2> intersects = new();
        IPlot plot;
        public bool useCross = true;

        public void SetParameter(IPlot plot, Vector2[] dataPositions = null)
        {
            float[] yIntervall = plot.axis.GetYAxisIntervall();
            float[] xIntervall = plot.axis.GetXAxisIntervall();
                this.xLeftBoundary = xIntervall[0];
                this.xRightBoundary = xIntervall[1];
                this.yLeftBoundary = yIntervall[0];
                this.yRightBoundary = yIntervall[1];
                zeroPosition = plot.axis.GetAxisZero2D();
                this.dataPositions = dataPositions;
                xIntervall = plot.axis.GetXAxisIntervall();
                yIntervall = plot.axis.GetYAxisIntervall();
                this.plot = plot;
                SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = lineColor;
            switch (type)
            {
                case ePlot2D.Line:
                    DrawLine(vh, vertex);
                    break;
                case ePlot2D.Scatter:
                    DrawScatter(vh, vertex);
                    break;
                default:
                    Debug.LogWarning(type + "plot 2D type not implemented");
                    break;
            }
        }
        private void DrawScatter(VertexHelper vh, UIVertex vertex)
        {
            dataPositions.ToList().ForEach(point =>
            {
                Vector2 center = plot.axis.TranslateToWorld(point);
                //UIVertex centerVertex = new UIVertex();
                UIVertex centerVertex = UIVertex.simpleVert;
                centerVertex.color = lineColor;
                centerVertex.position = center;
                if (!useCross)
                {
                    Vector2[] points = (CreatePoligon(center, lineStrength, 4));
                    vh.AddUIVertexQuad(CreateVerticies(points, vertex));

                }
                else
                {
                    DrawCrosses(vh, centerVertex, center);

                }

            });
        }
        void DrawCrosses(VertexHelper vh, UIVertex centerVertex, Vector2 center)
        {

            float x = center.x;
            float y = center.y;
            float r = lineStrength * 2;
            float s = 0.5f;
            Vector2[] positions = new Vector2[]
            {
                    new Vector2(x-r, y-r+s),
                    new Vector2(x-s, y),
                    new Vector2(x-r, y+r-s),
                    new Vector2(x-r+s, y+r),
                    new Vector2(x, y+s),
                    new Vector2(x+r-s, y+r),
                    new Vector2(x+r, y+r-s),
                    new Vector2(x+s, y),
                    new Vector2(x+r, y-r+s),
                    new Vector2(x+r-s, y-r),
                    new Vector2(x, y-s),
                    new Vector2(x-r+s, y-r)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                centerVertex.position = positions[i];
                vh.AddVert(centerVertex);
            }

            int count = vh.currentVertCount;
            vh.AddTriangle(count - 12, count - 11, count - 2);
            vh.AddTriangle(count - 2, count - 1, count - 12);
            vh.AddTriangle(count - 10, count - 9, count - 8);
            vh.AddTriangle(count - 8, count - 11, count - 10);
            vh.AddTriangle(count - 8, count - 7, count - 6);
            vh.AddTriangle(count - 6, count - 5, count - 8);
            vh.AddTriangle(count - 4, count - 3, count - 2);
            vh.AddTriangle(count - 2, count - 5, count - 4);
            vh.AddTriangle(count - 2, count - 11, count - 8);
            vh.AddTriangle(count - 8, count - 5, count - 2);
        }
        UIVertex[] CreateVerticies(Vector2[] pointsSorted, UIVertex origin)
        {
            UIVertex[] res = new UIVertex[pointsSorted.Length];
            for (int index = 0; index < pointsSorted.Length; index++)
            {
                UIVertex v = new UIVertex();
                v.color = origin.color;
                v.position = pointsSorted[index];

                res[index] = v;
            }
            return res;
        }
        Vector2[] CreatePoligon(Vector2 center, float radius, int side)
        {
            List<Vector2> points = new();
            float angleStepSize = (float)(2 * Math.PI) / side;
            float angle = 0;
            for (int index = 0; index < side; index++)
            {
                points.Add(new(
                        x: center.x + radius * Mathf.Sin(angle),
                        y: center.y + radius * Mathf.Cos(angle)
                    ));

                angle += angleStepSize;
            }
            return points.ToArray();
        }


        private void DrawLine(VertexHelper vh, UIVertex vertex)
        {
            intersects.Clear();
            bool first = true;
            Line lineSegment;
            Vector2 intersection = Vector2.zero;
            Vector2 point;
            List<Vector2> topPos = new();
            List<Vector2> bottomPos = new();
            Line pastTop = null;
            Line pastBottom = null;
            if (dataPositions.Count() < 2)
                return;
            for (int index = 1; index < dataPositions.Length; index++)
            {
                lineSegment = new(plot.axis.TranslateToWorld(dataPositions[index - 1]), plot.axis.TranslateToWorld(dataPositions[index]));


                point = lineSegment.start;

                Vector2 tPStart = point + (lineSegment.normal * lineStrength);
                Vector2 bPStart = point - (lineSegment.normal * lineStrength);



                point = lineSegment.end;
                Vector2 tPEnd = point + (lineSegment.normal * lineStrength);
                Vector2 bPEnd = point - (lineSegment.normal * lineStrength);

                Line bottom = new(bPStart, bPEnd);
                Line top = new(tPStart, tPEnd);
                if (pastBottom != null)
                {
                    bottom.start = lineSegment.start + lineStrength * GetAngleBisector(pastBottom.direction, bottom.direction);
                    bottomPos[^1] = bottom.start;
                }
                if (pastTop != null)
                {
                    top.start = lineSegment.start - lineStrength * GetAngleBisector(pastTop.direction, top.direction);
                    topPos[^1] = top.start;
                }
                bottom.Refresh();
                top.Refresh();

                if (first)
                {
                    topPos.Add(lineSegment.start + Vector2.up * lineStrength);
                    bottomPos.Add(lineSegment.start - Vector2.up * lineStrength);
                    first = false;
                }

                topPos.Add(lineSegment.end + Vector2.up * lineStrength);
                bottomPos.Add(lineSegment.end - Vector2.up * lineStrength);
                pastBottom = bottom; pastTop = top;


            }






            UIVertex v1 = UIVertex.simpleVert;
            v1.color = vertex.color;
            UIVertex v2 = UIVertex.simpleVert;
            v2.color = vertex.color;
            UIVertex v3 = UIVertex.simpleVert;
            v3.color = vertex.color;
            UIVertex v4 = UIVertex.simpleVert;
            v4.color = vertex.color;
            for (int index = 1; index < topPos.Count; index++)
            {
                v4.position = bottomPos[index];
                v3.position = topPos[index];
                v2.position = topPos[index - 1];
                v1.position = bottomPos[index - 1];
                vh.AddUIVertexQuad(new[] { v1, v2, v3, v4 });
            }
        }
        Vector2 GetAngleBisector(Vector2 dir1, Vector2 dir2)
        {
            Vector2 sum = dir1 + dir2;
            Vector2 bisector = new Vector2(-sum.y, sum.x) * -1;
            bisector.Normalize();
            return bisector;
        }

        private void OnDrawGizmos()
        {
            List<Vector2> points = new();
            if (dataPositions.Length < 1)
                return;
            dataPositions.ToList().ForEach(point => points.Add(transform.TransformPoint(point)));
            Gizmos.color = Color.yellow;
            for (int index = 0; index < points.Count - 1; index++)
            {
                Gizmos.DrawLine(transform.TransformPoint(points[index]), transform.TransformPoint(points[index + 1]));
                Gizmos.DrawWireSphere(transform.TransformPoint(points[index]), 15f);
            }
            Gizmos.DrawWireSphere(transform.TransformPoint(points[^1]), 15f);
        }

        public void SetColor(Color color)
        {
            color.a = 1f;
            lineColor = color;
        }
        public Color GetColor()
        {
            return lineColor;
        }
    }
}