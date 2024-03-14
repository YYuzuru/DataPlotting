using GraphPlotter.Interface;
using GraphPlotter.Utilities;
using HelperScirpts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphPlotter
{
    public class LinePlot : MonoBehaviour, IPlot2D
    {
        private LineGenerator lineGenerator;
        public List<Vector2> functionPoints = new();
        public IAxis2D axis { get; set; }
        public float density = 1;
        IAxis IPlot.axis => axis;
        public IFunction2D function { get; set; }
        IFunction IPlot.function => function;

        public static LinePlot Create(IFunction2D function, IAxis2D axis)
        {
            //AddComponent
            GameObject gameObject = new GameObject(function.name, typeof(CanvasRenderer), typeof(RectTransform), typeof(LineGenerator), typeof(LinePlot));
            LinePlot linePlot = gameObject.GetComponent<LinePlot>();
            linePlot.lineGenerator = gameObject.GetComponent<LineGenerator>();
            RectTransform rctTransform = linePlot.GetComponent<RectTransform>();
            
            gameObject.transform.SetParent(axis.gameObject.transform,false);
            RectEditing.CopyRectTransform(axis.gameObject.GetComponent<RectTransform>(), rctTransform);

            AssignAttributes(linePlot);

            linePlot.Refresh();

            return linePlot;

            void AssignAttributes(LinePlot linePlot)
            {
                linePlot.axis = axis;
                linePlot.function = function;
            }
        }

        void IPlot.Refresh() => Refresh();
        public ePlot2D plotType => lineGenerator.type;

        private void Refresh()
        {

            SetIntervall();
            


        }

        private void SetIntervall()
        {
            float[] xAxis = axis.GetXAxisIntervall();
            float start = xAxis[0];
            float end = xAxis[1];
            float step = (axis.GetStepSize().x /Mathf.Abs(start - end))*(Mathf.Abs(density)); // linear spacing

            functionPoints = function.GetIntervall(start, end, step, axis.useXLog);
            if(functionPoints.Count>0)
            //Debug.Log(functionPoints[0]+" => " + functionPoints[^1]);
            lineGenerator.SetParameter(this, functionPoints.ToArray());
        }


        void IColorable.SetColor(params Color[] color)
        {
            if (color.Length > 1)
                Debug.LogWarning("LinePlot does not support multicolor");

            lineGenerator.SetColor(color[0]);
        }
        Color[] IColorable.GetColor()
        {
            return new Color[] { lineGenerator.GetColor() };
        }

        public void SetPlotType(int type)
        {
            this.lineGenerator.type = (ePlot2D)type;
        }
        public int GetPlotType()
        {
            return (int)lineGenerator.type;
        }

        public Vector2 GetSignal(float position)
        {
            switch (plotType)
            {
                case ePlot2D.Line:
                    return function.GetSignal(position);
                case ePlot2D.Scatter:
                    return GetSignalClosest(position);
                default:
                    throw new NotImplementedException();
            }
        }

        Vector2 GetSignalClosest(float position)
        {
            float distance = float.PositiveInfinity;
            Vector2 signal = Vector2.negativeInfinity;

            for (int index = 0; index < functionPoints.Count; index++)
            {
                if (Mathf.Abs(position - functionPoints[index].x) > distance)
                    continue;
                distance = Mathf.Abs(position - functionPoints[index].x);
                signal = functionPoints[index];
            }
            return signal;
        }

        public void SetLineStrength(float strength)
        {
            lineGenerator.lineStrength = strength;
        }
        public float GetLineStrength()
        {
            return lineGenerator.lineStrength;
        }

        public void SetPointDensity(float density)
        {
            this.density = density;
        }
        public float GetPointDensity()
        {
            return this.density;
        }

        public override bool Equals(object obj)
        {
            return obj is LinePlot plot &&
                   base.Equals(obj) &&
                   density == plot.density;
        }
    }
}