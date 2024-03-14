using GraphPlotter.Interface;
using HelperScirpts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DataGenerator;

namespace GraphPlotter
{
    public class HeatMap : MonoBehaviour, IPlot3D
    {
        public Color[] color = new[] { Color.blue, Color.red };
        private HeatMapTextureGenerator heatMapTextureGenerator;
        public IAxis axis { get; set; }
        public IFunction3D function { get; set; }

        IFunction IPlot.function => function;

        public static HeatMap Create(IFunction3D function, IAxis3D axis)
        {
            //AddComponent
            GameObject gameObject = new GameObject(function.name, typeof(CanvasRenderer), typeof(RectTransform), typeof(HeatMap), typeof(HeatMapTextureGenerator), typeof(RawImage));
            HeatMap heatMap = gameObject.GetComponent<HeatMap>();
            heatMap.heatMapTextureGenerator = gameObject.GetComponent<HeatMapTextureGenerator>();
            RectTransform rctTransform = heatMap.GetComponent<RectTransform>();

            gameObject.transform.SetParent(axis.gameObject.transform,false);
            RectEditing.CopyRectTransform(axis.gameObject.GetComponent<RectTransform>(), rctTransform);

            AssignAttributes(heatMap);

            return heatMap;

            void AssignAttributes(HeatMap heatMap)
            {
                heatMap.axis = axis;
                heatMap.function = function;
            }
        }

        public ePlot3D plotType => heatMapTextureGenerator.type;

        public Color[] GetColor()
        {
            return color;
        }

        public void Refresh()
        {
            float[] xIntervall = axis.GetXAxisIntervall();
            float[] yIntervall = axis.GetYAxisIntervall();
            Vector2 start = new Vector2(xIntervall[0], yIntervall[0]);
            Vector2 end = new Vector2(xIntervall[^1], yIntervall[^1]);

            SetIntervall(start, end, Vector2.one / 10);

        }

        public void SetColor(params Color[] color)
        {
            heatMapTextureGenerator.SetColor(color);
        }

        public void SetIntervall(Vector2 start, Vector2 end, Vector2 stepSize)
        {
            DataPoint[,] data = function.GetIntervall();
            heatMapTextureGenerator.GenerateHeatMapTexture(axis, data);
        }

        public void SetPlotType(int type)
        {
            this.heatMapTextureGenerator.type = (ePlot3D)type;
        }
        public int GetPlotType()
        {
            return (int)heatMapTextureGenerator.type;
        }

        public Vector3 GetSignal(Vector2 position) => function.GetSignal(position);
    }

}