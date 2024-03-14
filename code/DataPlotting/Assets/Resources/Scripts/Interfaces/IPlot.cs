using UnityEngine;

namespace GraphPlotter.Interface
{
    public interface IPlot : IColorable
    {
        IAxis axis { get; }
        GameObject gameObject { get; }
        IFunction function { get; }
        void Refresh();
        /// <summary>
        /// take ePlot2D or ePlot3D as reference
        /// </summary>
        /// <param name="plotType"></param>
        void SetPlotType(int plotType);
        int GetPlotType();
    }

    public interface IPlot2D : IPlot
    {
        ePlot2D plotType { get; }
        new IFunction2D function { get; }

        Vector2 GetSignal(float position);
        void SetLineStrength(float strength);
        float GetLineStrength();

        void SetPointDensity(float density);
        float GetPointDensity();


    }
    public interface IPlot3D : IPlot
    {
        ePlot3D plotType { get; }
        new IFunction3D function { get; }
        void SetIntervall(Vector2 start, Vector2 end, Vector2 stepSize);
        Vector3 GetSignal(Vector2 position);

    }

}