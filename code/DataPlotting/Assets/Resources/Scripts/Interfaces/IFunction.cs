using System;
using System.Collections.Generic;
using UnityEngine;
using static DataGenerator;

namespace GraphPlotter.Interface
{
    public interface IFunction
    {
        string name { get; set; }
    }
    public interface IFunction2D: IFunction
    {
        Vector2[] max { get; }
        Vector2? maxGlobal { get; }
        Vector2[] min { get; }
        Vector2? minGlobal { get; }
        List<Vector2> GetIntervall(float start, float end, float step,bool useLog);
        Vector2 GetSignal(float position);
    }

    public interface IFunction3D: IFunction
    {
        Vector3[] max { get; }
        Vector3? maxGlobal { get; }
        Vector3[] min { get; }
        Vector3? minGlobal { get; }
        DataPoint[,] GetIntervall();
        Vector3 GetSignal(Vector2 position);
    }
}