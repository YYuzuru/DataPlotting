using System.Collections.Generic;
using UnityEngine;

namespace GraphPlotter.Interface
{
    public interface IAxis
    {
        IGraph graph { get; }
        GameObject gameObject { get; }

        /// <summary>
        /// zero-Point of axis in world space
        /// </summary>
        /// <returns>(0,0) for linear axis and (10^0,10^0) for logarithmic axis </returns>
        Vector3 GetAxisZero();
        /// <summary>
        /// zero-Point of axis in canvas space
        /// </summary>
        /// <returns>(0,0) for linear axis and (10^0,10^0) for logarithmic axis</returns>
        Vector2 GetAxisZero2D();

        /// <summary>
        /// Calculate the occupied world space of the target space (e.g axis-canvasSpace) and divide it by the intervall-distance of the axis
        /// </summary>
        /// <returns>skala of axis to world </returns>
        Vector2 GetScale();

        /// <summary>
        /// corners of axis in world space
        /// </summary>
        ///  /// <returns>Vector2D-array of size 4 with index=0 being bottom left, index = 1: top left, index=2: top right, index=3: bottom right</returns>
        Vector2[] GetAxisWorldCorners();
        /// <summary>
        /// corners of axis in local space
        /// </summary>
        /// <returns>Vector2D-array of size 4 with index=0 being bottom left, index = 1: top left, index=2: top right, index=3: bottom right</returns>
        Vector2[] GetAxisCorners();
        /// <summary>
        ///  offfset to parent canvas
        /// </summary>
        /// <returns>Vector-array of size 2 with index=0: bottom left, index = 1: top right</returns>
        Vector2[] GetAxisOffset();

        /// <summary>
        /// Same as GetYAxisIntervall()
        /// x-axis intervall. It will not auto transform for logarithmic axis from 0 to 10^0 = 1. From these options 0 will be returned
        /// </summary>
        /// <returns>start: [0] ; end: [1]</returns>
        float[] GetXAxisIntervall();

        /// <summary>
        /// Same as GetXAxisIntervall()
        /// y-axis intervall. It will not auto transform for logarithmic axis from 1 to 10^1 = 10. From these options 1 will be returned
        /// </summary>
        /// <returns>start: [0] ; end: [1]</returns>
        float[] GetYAxisIntervall();

        /// <summary>
        /// Distance between each tick/step
        /// </summary>
        /// <returns>Cellsize from one tick to another.</returns>
        Vector2 GetStepSize();

        /// <summary>
        /// Area avaible for plots to be drawn on
        /// </summary>
        Vector2 GetAxisSize()
        {
            Vector2[] corners = GetAxisWorldCorners();
            //Vector2 size = corners[2] - corners[0];
            Vector2 size = new(x: corners[3].x - corners[0].x, y: corners[1].y - corners[0].y);

            return size;
        }
        /// <summary>
        /// Transforms coordinate of a plot into the local-World-position. To get the global position (e.g. for Gizmos.Draw purposes). Apply axis.gameobject.transform.TransformPoint(Vector2 local-coordinate) after using this function
        /// To inverse, use TranslateToAxis(Vector3 point)
        /// </summary>
        /// <param name="point"> coordinate of the plot-point</param>
        /// <returns>local coordinate of the point of a plot</returns>
        Vector3 TranslateToWorld(Vector2 point);
        /// <summary>
        /// Transforms coordinate of World-position into axis-coordinate. You might need to transform the global-world-position to a local one using gameobject.transform.InverseTranformPoint(Vector3). (0,0) will be considered the axis-zero
        /// To inverse, use TranslateToWorld(Vector2 point)
        /// </summary>
        /// <param name="point"> coordinate of the point</param>
        /// <returns>plot-coordinate</returns>
        Vector2 TranslateToAxis(Vector3 point);

        /// <summary>
        /// axis is use-ready
        /// </summary>
        bool isActive { get; }

    }
    public interface IAxis2D:IAxis
    {
        List<IPlot2D> plotList { get; }
        new IGraph2D graph { get; set; }
        IGraph IAxis.graph => graph;
        IPlot2D AddPlot(IFunction2D function);

        bool useXLog { get; }
        bool useYLog { get; }

    }
    public interface IAxis3D:IAxis
    {
        List<IPlot3D> plotList { get; }
        new IGraph3D graph { get; set; }
        IGraph IAxis.graph => graph;

        IPlot3D AddPlot(IFunction3D function);
    }
}