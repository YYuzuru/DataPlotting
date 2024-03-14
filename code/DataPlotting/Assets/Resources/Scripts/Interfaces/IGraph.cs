using UnityEngine;

namespace GraphPlotter.Interface
{
    public interface IGraph2D: IGraph
    {
        new IAxis2D axis { get; }
        IAxis IGraph.axis => axis;
        GameObject gameObject { get; }
        IPlot2D AddPlot(IFunction2D function,params Color[] color); //returns created plot

    }
    public interface IGraph3D: IGraph
    {
        new IAxis3D axis { get; }
        IAxis IGraph.axis => axis;
        GameObject gameObject { get; }
        IPlot3D AddPlot(IFunction3D function,params Color[] color); //returns created plot
    }
    public interface IGraph
    {
        IAxis axis { get; }
        RectTransform rectTransform { get; }
        IPlot[] GetActivePlots();
    }
}