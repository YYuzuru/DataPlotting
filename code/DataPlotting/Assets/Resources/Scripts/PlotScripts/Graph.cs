using GraphPlotter.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace GraphPlotter
{
    public class Graph : MonoBehaviour, IGraph2D, IGraph3D
    {
        IAxis2D axis2D;
        IAxis3D axis3D;

        public RectTransform rectTransform { get; private set; }
        IAxis2D IGraph2D.axis => axis2D;
        IAxis3D IGraph3D.axis => axis3D;

        IAxis IGraph.axis => axis3D;

        public IPlot2D AddPlot(IFunction2D function, params Color[] color)
        {
            if (axis2D == null)
                Debug.LogWarning("Graph is missing an axis", transform.gameObject);
            IPlot2D plot = axis2D.AddPlot(function);
            plot.SetColor(color);
            plot.Refresh();
            return plot;
        }
        public IPlot3D AddPlot(IFunction3D function, params Color[] color)
        {
            if (axis3D == null)
                Debug.LogWarning("Graph is missing an axis", transform.gameObject);
            IPlot3D plot = axis3D.AddPlot(function);
            plot.SetColor(color);
            //plot.Refresh();
            return plot;
        }


        public void OnDrawGizmos()
        {
            if (true || rectTransform == null)
                return;
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            foreach (Vector3 cor in corners)
            {
                Gizmos.DrawCube(cor, Vector3.one * 40);
            }
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            axis2D = this.GetComponentInChildren<IAxis2D>();
            axis2D.graph = this;
            axis3D = this.GetComponentInChildren<IAxis3D>();
            axis3D.graph = this;

        }


        public IPlot[] GetActivePlots()
        {
            List<IPlot> activePlots = new();
            activePlots.AddRange(axis2D.plotList.FindAll(plot => plot.gameObject.activeSelf));
            activePlots.AddRange(axis3D.plotList.FindAll(plot => plot.gameObject.activeSelf));

            return activePlots.ToArray();
        }

    }
}