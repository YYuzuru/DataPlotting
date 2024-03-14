using GraphPlotter.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GraphPlotter
{
    public class AxisScript : Graphic
    {
        public ControllerScript controller;
        private IAxis axis;
        private IGraph graph;
        private float canvasHeight;
        private float canvasWidth;
        public Vector2 canvasSize => new(x: canvasWidth, y: canvasHeight);
        public bool singleSectorAxis;
        public bool forceSquare;
        private int vertCount;


        private bool xZeroIsInRange;
        private bool yZeroIsInRange;

        public Vector2 intersectionPosition;

        public Vector2 xDrawStartPosition;
        public Vector2 xDrawEndPosition;

        public Vector2 yDrawStartPosition;
        public Vector2 yDrawEndPosition;

        public Vector2 xCalculatedStartPosition;
        public Vector2 xCalculatedEndPosition;

        public Vector2 yCalculatedStartPosition;
        public Vector2 yCalculatedEndPosition;

        public float xCalculatedLength;
        public float yCalculatedLength;

        public float xValueRange;
        public float yValueRange;

        private int xNumberOfTicks;
        private int yNumberOfTicks;

        public float xSpaceBetweenTicks;
        public float ySpaceBetweenTicks;

        public Vector2[] xTickPositions;
        public Vector2[] yTickPositions;

        private RectTransform xLeftInputField;
        private RectTransform xRightInputField;
        private RectTransform yLeftInputField;
        private RectTransform yRightInputField;

        public float leftSpace;
        public float rightSpace;
        public float topSpace;
        public float bottomSpace;

        public float xLeftBoundary;
        public float xRightBoundary;
        public float yLeftBoundary;
        public float yRightBoundary;

        public float xStepSize;
        public float yStepSize;

        public float lineStrength;
        public float tickWidth;
        public float tickHeight;
        public int tickPosition;

        public bool redrawAxis;
        public bool redrawTickLabels;
        protected override void Awake()
        {
            graph = transform.parent.GetComponent<IGraph>();
            axis = graph.axis;
            canvasWidth = graph.rectTransform.rect.width;
            canvasHeight = graph.rectTransform.rect.height;

            transform.parent.transform.GetComponent<IGraph>();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (redrawAxis)
            {
                vh.Clear();

                UIVertex vertex = UIVertex.simpleVert;
                vertex.color = color;
                if (singleSectorAxis) CalculateStartAndEndPositionsForSingleSector();
                else CalculateStartAndEndPositionsForMultiSector();

                CalculateLinearXTickPositions();

                CalculateLinearYTickPositions();


                DrawAxis(vh, vertex);
                DrawTicks(vh, vertex);
                redrawTickLabels = true;
                //PositionInputFields();
                redrawAxis = false;
            }
        }

        private void CalculateStartAndEndPositionsForMultiSector()
        {
            xZeroIsInRange = (xLeftBoundary <= 0 && xRightBoundary >= 0);
            yZeroIsInRange = (yLeftBoundary <= 0 && yRightBoundary >= 0);

            xDrawStartPosition = new Vector2(leftSpace - lineStrength, 0);
            xDrawEndPosition = new Vector2(canvasWidth - rightSpace + lineStrength, 0);
            xCalculatedStartPosition = new Vector2(leftSpace, 0);
            xCalculatedEndPosition = new Vector2(canvasWidth - rightSpace, 0);

            yDrawStartPosition = new Vector2(0, bottomSpace - lineStrength);
            yDrawEndPosition = new Vector2(0, canvasHeight - topSpace + lineStrength);
            yCalculatedStartPosition = new Vector2(0, bottomSpace);
            yCalculatedEndPosition = new Vector2(0, canvasHeight - topSpace);

            xCalculatedLength = Mathf.Abs(xCalculatedEndPosition.x - xCalculatedStartPosition.x);
            yCalculatedLength = Mathf.Abs(yCalculatedEndPosition.y - yCalculatedStartPosition.y);

            xValueRange = Mathf.Abs(xRightBoundary - xLeftBoundary);
            yValueRange = Mathf.Abs(yRightBoundary - yLeftBoundary);

            if (xZeroIsInRange)
            {
                if (xLeftBoundary == 0) intersectionPosition.x = xCalculatedStartPosition.x;
                else if (xRightBoundary == 0) intersectionPosition.x = xCalculatedEndPosition.x;
                else intersectionPosition.x = xCalculatedStartPosition.x + xCalculatedLength * Mathf.Abs(xLeftBoundary / xValueRange);
                yCalculatedStartPosition.x = intersectionPosition.x;
                yCalculatedEndPosition.x = intersectionPosition.x;
                yDrawStartPosition.x = intersectionPosition.x;
                yDrawEndPosition.x = intersectionPosition.x;
            }
            else
            {
                yCalculatedStartPosition.x = xCalculatedStartPosition.x;
                yCalculatedEndPosition.x = xCalculatedStartPosition.x;
                yDrawStartPosition.x = xCalculatedStartPosition.x;
                yDrawEndPosition.x = xCalculatedStartPosition.x;
            }

            if (yZeroIsInRange)
            {
                if (yLeftBoundary == 0) intersectionPosition.y = yCalculatedStartPosition.y;
                else if (yRightBoundary == 0) intersectionPosition.y = xCalculatedEndPosition.y;
                else intersectionPosition.y = yCalculatedStartPosition.y + yCalculatedLength * Mathf.Abs(yLeftBoundary / yValueRange);
                xCalculatedStartPosition.y = intersectionPosition.y;
                xCalculatedEndPosition.y = intersectionPosition.y;
                xDrawStartPosition.y = intersectionPosition.y;
                xDrawEndPosition.y = intersectionPosition.y;
            }
            else
            {
                xCalculatedStartPosition.y = yCalculatedStartPosition.y;
                xCalculatedEndPosition.y = yCalculatedStartPosition.y;
                xDrawStartPosition.y = yCalculatedStartPosition.y;
                xDrawEndPosition.y = yCalculatedStartPosition.y;
            }
            if ((((IAxis2D)axis).useXLog))
            {
            }
            if (forceSquare)
            {
                xCalculatedEndPosition.x = xCalculatedStartPosition.x + yCalculatedLength;
                xDrawEndPosition.x = xDrawStartPosition.x + yCalculatedLength + 2 * lineStrength;
                xCalculatedLength = Mathf.Abs(xCalculatedEndPosition.x - xCalculatedStartPosition.x);
            }
        }

        private void CalculateStartAndEndPositionsForSingleSector()
        {
            xDrawStartPosition = new Vector2(leftSpace - lineStrength, bottomSpace);
            xDrawEndPosition = new Vector2(canvasWidth - rightSpace + lineStrength, bottomSpace);
            xCalculatedStartPosition = new Vector2(leftSpace, bottomSpace);
            xCalculatedEndPosition = new Vector2(canvasWidth - rightSpace, bottomSpace);

            yDrawStartPosition = new Vector2(leftSpace, bottomSpace - lineStrength);
            yDrawEndPosition = new Vector2(leftSpace, canvasHeight - topSpace + lineStrength);
            yCalculatedStartPosition = new Vector2(leftSpace, bottomSpace);
            yCalculatedEndPosition = new Vector2(leftSpace, canvasHeight - topSpace);

            xCalculatedLength = Mathf.Abs(xCalculatedEndPosition.x - xCalculatedStartPosition.x);
            yCalculatedLength = Mathf.Abs(yCalculatedEndPosition.y - yCalculatedStartPosition.y);

            xValueRange = Mathf.Abs(xRightBoundary - xLeftBoundary);
            yValueRange = Mathf.Abs(yRightBoundary - yLeftBoundary);

            if (forceSquare)
            {
                xCalculatedEndPosition.x = xCalculatedStartPosition.x + yCalculatedLength;
                xDrawEndPosition.x = xDrawStartPosition.x + yCalculatedLength + 2 * lineStrength;
                xCalculatedLength = Mathf.Abs(xCalculatedEndPosition.x - xCalculatedStartPosition.x);
            }

            if (xCalculatedStartPosition == yCalculatedStartPosition) intersectionPosition = xCalculatedStartPosition;
            else Debug.LogError("xCalculatedStartPosition does not equal yCalculatedStartPosition");
        }

        private void DrawAxis(VertexHelper vh, UIVertex vertex)
        {
            vertex.position = new Vector2(xDrawStartPosition.x, xDrawStartPosition.y - lineStrength);
            vh.AddVert(vertex);
            vertex.position = new Vector2(xDrawStartPosition.x, xDrawStartPosition.y + lineStrength);
            vh.AddVert(vertex);
            vertex.position = new Vector2(xDrawEndPosition.x, xDrawStartPosition.y + lineStrength);
            vh.AddVert(vertex);
            vertex.position = new Vector2(xDrawEndPosition.x, xDrawStartPosition.y - lineStrength);
            vh.AddVert(vertex);

            vertCount = vh.currentVertCount;
            vh.AddTriangle(vertCount - 4, vertCount - 3, vertCount - 2);
            vh.AddTriangle(vertCount - 2, vertCount - 1, vertCount - 4);

            vertex.position = new Vector2(yDrawStartPosition.x - lineStrength, yDrawStartPosition.y);
            vh.AddVert(vertex);
            vertex.position = new Vector2(yDrawEndPosition.x - lineStrength, yDrawEndPosition.y);
            vh.AddVert(vertex);
            vertex.position = new Vector2(yDrawEndPosition.x + lineStrength, yDrawEndPosition.y);
            vh.AddVert(vertex);
            vertex.position = new Vector2(yDrawStartPosition.x + lineStrength, yDrawStartPosition.y);
            vh.AddVert(vertex);

            vertCount = vh.currentVertCount;
            vh.AddTriangle(vertCount - 4, vertCount - 3, vertCount - 2);
            vh.AddTriangle(vertCount - 2, vertCount - 1, vertCount - 4);
        }

        private void CalculateLinearXTickPositions()
        {
            if (xStepSize != 0) xNumberOfTicks = Mathf.FloorToInt(xValueRange / xStepSize) + 1;
            xSpaceBetweenTicks = xCalculatedLength / (xValueRange * (1 / xStepSize));
            xTickPositions = new Vector2[xNumberOfTicks];

            for (int i = 0; i < xNumberOfTicks; i++)
            {
                xTickPositions[i] = new Vector2(xCalculatedStartPosition.x + i * xSpaceBetweenTicks, xCalculatedStartPosition.y);
            }

            if (xNumberOfTicks < 2)
            {
                xNumberOfTicks = 1;
                xTickPositions[0] = xDrawStartPosition;
            }
        }

        private void CalculateLinearYTickPositions()
        {
            if (yStepSize != 0) yNumberOfTicks = Mathf.FloorToInt(yValueRange / yStepSize) + 1;
            ySpaceBetweenTicks = yCalculatedLength / (yValueRange * (1 / yStepSize));
            if (yNumberOfTicks < 2) yNumberOfTicks = 2;
            yTickPositions = new Vector2[yNumberOfTicks];

            for (int i = 0; i < yNumberOfTicks; i++)
            {
                yTickPositions[i] = new Vector2(yCalculatedStartPosition.x, yCalculatedStartPosition.y + i * ySpaceBetweenTicks);
            }

            if (yNumberOfTicks < 2)
            {
                yNumberOfTicks = 1;
                yTickPositions[0] = yDrawStartPosition;
            }
        }

        private void DrawTicks(VertexHelper vh, UIVertex vertex)
        {
            for (int i = 0; i < xNumberOfTicks; i++)
            {
                float offset;
                if (tickPosition == -1) offset = -tickHeight;
                else if (tickPosition == 1) offset = tickHeight;
                else offset = 0;

                Vector2 pos = xTickPositions[i];

                vertex.position = new Vector2(pos.x - tickWidth, pos.y - tickHeight + offset);
                vh.AddVert(vertex);
                vertex.position = new Vector2(pos.x - tickWidth, pos.y + tickHeight + offset);
                vh.AddVert(vertex);
                vertex.position = new Vector2(pos.x + tickWidth, pos.y + tickHeight + offset);
                vh.AddVert(vertex);
                vertex.position = new Vector2(pos.x + tickWidth, pos.y - tickHeight + offset);
                vh.AddVert(vertex);

                vertCount = vh.currentVertCount;
                vh.AddTriangle(vertCount - 4, vertCount - 3, vertCount - 2);
                vh.AddTriangle(vertCount - 2, vertCount - 1, vertCount - 4);
            }

            for (int i = 0; i < yNumberOfTicks; i++)
            {
                float offset;
                if (tickPosition == -1) offset = -tickHeight;
                else if (tickPosition == 1) offset = tickHeight;
                else offset = 0;

                Vector2 pos = yTickPositions[i];

                vertex.position = new Vector2(pos.x - tickHeight + offset, pos.y - tickWidth);
                vh.AddVert(vertex);
                vertex.position = new Vector2(pos.x - tickHeight + offset, pos.y + tickWidth);
                vh.AddVert(vertex);
                vertex.position = new Vector2(pos.x + tickHeight + offset, pos.y + tickWidth);
                vh.AddVert(vertex);
                vertex.position = new Vector2(pos.x + tickHeight + offset, pos.y - tickWidth);
                vh.AddVert(vertex);

                vertCount = vh.currentVertCount;
                vh.AddTriangle(vertCount - 4, vertCount - 3, vertCount - 2);
                vh.AddTriangle(vertCount - 2, vertCount - 1, vertCount - 4);
            }
        }


        private void PositionInputFields()
        {
            xLeftInputField = GameObject.Find("xLeftValue").GetComponent<RectTransform>();
            xRightInputField = GameObject.Find("xRightValue").GetComponent<RectTransform>();
            yLeftInputField = GameObject.Find("yLeftValue").GetComponent<RectTransform>();
            yRightInputField = GameObject.Find("yRightValue").GetComponent<RectTransform>();

            Vector2 bottomLeft = new Vector2(-canvasWidth / 2, -canvasHeight / 2);
            /*
            xLeftInputField.anchoredPosition = new Vector2(bottomLeft.x + xTickPositions[0].x, bottomLeft.y + xTickPositions[0].y);
            xRightInputField.anchoredPosition = new Vector2(bottomLeft.x + xTickPositions[xNumberOfTicks - 1].x, bottomLeft.y + xTickPositions[xNumberOfTicks - 1].y);
            yLeftInputField.anchoredPosition = new Vector2(bottomLeft.x + yTickPositions[0].x, bottomLeft.y + yTickPositions[0].y);
            yRightInputField.anchoredPosition = new Vector2(bottomLeft.x + yTickPositions[yNumberOfTicks - 1].x, bottomLeft.y + yTickPositions[yNumberOfTicks -1].y);
            */
            xLeftInputField.anchoredPosition = new Vector2(bottomLeft.x + xTickPositions[0].x, bottomLeft.y + xTickPositions[0].y);
            xRightInputField.anchoredPosition = new Vector2(bottomLeft.x + xTickPositions[xNumberOfTicks - 1].x, bottomLeft.y + xTickPositions[xNumberOfTicks - 1].y);
            yLeftInputField.anchoredPosition = new Vector2(bottomLeft.x + 100, bottomLeft.y + yTickPositions[0].y);
            yRightInputField.anchoredPosition = new Vector2(bottomLeft.x + yTickPositions[yNumberOfTicks - 1].x, bottomLeft.y + yTickPositions[yNumberOfTicks - 1].y);
        }
        public void RedrawAxis()
        {

            xLeftBoundary = axis.GetXAxisIntervall()[0];
            xRightBoundary = axis.GetXAxisIntervall()[1];
            yLeftBoundary = axis.GetYAxisIntervall()[0];
            yRightBoundary = axis.GetYAxisIntervall()[1];

            xStepSize = axis.GetStepSize().x;
            yStepSize = axis.GetStepSize().y;

            redrawAxis = true;
            SetVerticesDirty();
        }


        public Vector2 GetAxisSize() => new Vector2(xCalculatedLength, yCalculatedLength);
        public Vector2 GetZeroZeroPosition() //=> new Vector2(x: xCalculatedStartPosition.x, y: yCalculatedStartPosition.y) - new Vector2(x: xLeftBoundary, y: yLeftBoundary);//new Vector2(xCalculatedStartPosition.x, yCalculatedStartPosition.y);
        {
            Vector2 offsetMin = intersectionPosition;
            /*if(!controller.useXLog)*/
            offsetMin.x = xCalculatedStartPosition.x + ((0 - xLeftBoundary) / xValueRange) * xCalculatedLength;
            /*if (!controller.useYLog)*/
            offsetMin.y = yCalculatedStartPosition.y + ((0 - yLeftBoundary) / yValueRange) * yCalculatedLength;
            return offsetMin;
        }
        
    }

}