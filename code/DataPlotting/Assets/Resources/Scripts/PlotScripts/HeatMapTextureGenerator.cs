using GraphPlotter.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using static DataGenerator;

namespace GraphPlotter
{
    public class HeatMapTextureGenerator : MonoBehaviour
    {
        public DataGenerator.DataPoint[,] inputData;
        DataGenerator.DataPoint[,] useableData;

        Texture2D texture;
        RawImage image;
        public ePlot3D type = ePlot3D.HeatMap;
        public AxisScript axisScript;
        public IAxis axis;
        private float xLeftBoundary;
        private float xRightBoundary;
        private float yLeftBoundary;
        private float yRightBoundary;

        private int width;
        private int height;

        private float xMinValue;
        private float xMaxValue;
        private float yMinValue;
        private float yMaxValue;

        public Gradient gradient = new();
        public void SetColor(params Color[] color)
        {
            Gradient gradient = new Gradient();
            float stepSize = (float)Math.Round((double)(1f / (color.Length - 1)), 3);


            GradientColorKey[] colorKeyList = new GradientColorKey[color.Length];
            GradientAlphaKey[] alphaKeyList = new GradientAlphaKey[color.Length];
            for (int index = 0; index < color.Length; index++)
            {
                GradientColorKey colorKey = new GradientColorKey(new(r: color[index].r, g: color[index].g, b: color[index].b), stepSize * index);
                GradientAlphaKey alphaKey = new GradientAlphaKey(color[index].a, stepSize * index);
                colorKeyList[index] = colorKey;
                alphaKeyList[index] = alphaKey;
            }
            gradient.SetKeys(colorKeyList, alphaKeyList);
            this.gradient = gradient;

            //Debug.Log(string.Join(", ", color)+" => "+string.Join(",",colorKeyList));
        }

        public void GenerateHeatMapTexture(IAxis axis, DataPoint[,] input = null)
        {

            float[] yIntervall = axis.GetYAxisIntervall();
            float[] xIntervall = axis.GetXAxisIntervall();

            this.xLeftBoundary = xIntervall[0];
            this.xRightBoundary = xIntervall[1];
            this.yLeftBoundary = yIntervall[0];
            this.yRightBoundary = yIntervall[1];

            this.inputData = input;
            this.axis = axis;

            ExtractSignalValues();
            texture = GenerateTexture();
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            image = this.GetComponent<RawImage>();
            SetImageRectTransform();
            image.texture = texture;
            CalculateOffsets();
        }
        void ExtractSignalValues()
        {
            int xStart = 0;
            int xEnd = inputData.GetLength(0) - 1;
            int yStart = 0;
            int yEnd = inputData.GetLength(1) - 1;

            for (int i = 0; i < inputData.GetLength(0); i++)
            {
                if (inputData[i, 0].x >= xLeftBoundary)
                {
                    xStart = i;
                    break;
                }
            }

            for (int i = inputData.GetLength(0) - 1; i >= 0; i--)
            {
                if (inputData[i, 0].x <= xRightBoundary)
                {
                    xEnd = i;
                    break;
                }
            }

            for (int i = 0; i < inputData.GetLength(1); i++)
            {
                if (inputData[0, i].y >= yLeftBoundary)
                {
                    yStart = i;
                    break;
                }
            }

            for (int i = inputData.GetLength(1) - 1; i >= 0; i--)
            {
                if (inputData[0, i].y <= yRightBoundary)
                {
                    yEnd = i;
                    break;
                }
            }
            int xSize = Mathf.Abs(xStart - xEnd) + 1;
            int ySize = Mathf.Abs(yStart - yEnd) + 1;

            useableData = new DataPoint[xSize, ySize];
            int xCounter = 0;
            int yCounter = 0;
            for (int i = xStart; i <= xEnd; i++)
            {
                for (int j = yStart; j <= yEnd; j++)
                {
                    useableData[xCounter, yCounter] = inputData[i, j];
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
        }

        Texture2D GenerateTexture()
        {
            width = useableData.GetLength(0);
            height = useableData.GetLength(1);

            texture = new Texture2D(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    texture.SetPixel(i, j, gradient.Evaluate(useableData[i, j].signal));
                }
            }
            return texture;
        }

        private void CalculateOffsets()
        {
            if (axis == null)
                Debug.LogWarning("axis = null");
            xMinValue = useableData[0, 0].x;
            xMaxValue = useableData[useableData.GetLength(0) - 1, 0].x;
            yMinValue = useableData[0, 0].y;
            yMaxValue = useableData[0, useableData.GetLength(1) - 1].y;

            Vector2 xStartPoint = axis.GetAxisOffset()[0];//axisScript.xCalculatedStartPosition;
            Vector2 yStartPoint = axis.GetAxisOffset()[0]; //axisScript.yCalculatedStartPosition;
            float xValueRange = Mathf.Abs(xRightBoundary - xLeftBoundary);
            float yValueRange = Mathf.Abs(yRightBoundary - yLeftBoundary);
            float xLength = Math.Abs(axis.GetAxisOffset()[1].x - axis.GetAxisOffset()[0].x); //axisScript.xCalculatedLength;
            float yLength = Math.Abs(axis.GetAxisOffset()[1].y - axis.GetAxisOffset()[0].y); //axisScript.yCalculatedLength;

            float xMinPos = xStartPoint.x + ((xMinValue - xLeftBoundary) / xValueRange) * xLength;
            float imageWidth = (xStartPoint.x + ((xMaxValue - xLeftBoundary) / xValueRange) * xLength) - xMinPos;
            //Debug.Log("xMinValue" + xMinValue);
            //Debug.Log("xMaxValue" + xMaxValue);
            //Debug.Log("xLeftBoundary" + xLeftBoundary);
            //Debug.Log("xValueRange" + xValueRange);
            //Debug.Log("xLength" + xLength);
            //Debug.Log("xMinPos" + xMinPos);
            //Debug.Log("\n");
            float yMinPos = yStartPoint.y + ((yMinValue - yLeftBoundary) / yValueRange) * yLength;
            float imageHeight = (yStartPoint.y + ((yMaxValue - yLeftBoundary) / yValueRange) * yLength) - yMinPos;

            Vector2 offsetMin = new Vector2(xMinPos, yMinPos);
            Vector2 imageDimensions = new Vector2(imageWidth, imageHeight);

            this.GetComponent<RectTransform>().offsetMin = offsetMin;
            //this.GetComponent<RectTransform>().offsetMax = offsetMax;
            this.GetComponent<RectTransform>().sizeDelta = imageDimensions;
        }
        void SetImageRectTransform()
        {
            RectTransform rect = image.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
        }
    }
}
