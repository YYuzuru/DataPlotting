using GraphPlotter.Interface;
using GraphPlotter.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphPlotter
{
    public class Function : IFunction2D
    {
        public Vector2[] _max = new Vector2[0];
        public Vector2? _maxGlobal = new Vector2(float.MinValue, float.MinValue);
        public Vector2[] _min = new Vector2[0];
        public Vector2? _minGlobal = new Vector2(float.MaxValue, float.MaxValue);

        public Function(Func<float, float> function, string name = null)
        {
            this.function = function;
            this.name = name ?? this.function.Method.Name;
        }

        public Func<float, float> function { get; private set; }
        Vector2[] IFunction2D.max => _max;
        Vector2? IFunction2D.maxGlobal => _maxGlobal;
        Vector2[] IFunction2D.min => _min;
        Vector2? IFunction2D.minGlobal => _minGlobal;
        public string name { get; set; }

        List<Vector2> IFunction2D.GetIntervall(float start, float end,float step, bool useLog)
        {
            List<Vector2> points = new();
            if (start >= end)
                return points;
            points = (!useLog) ? FunctionSeries.GetLinear(function, start, end, step) : FunctionSeries.GetLog10(function, start, end, step);
            (_min, _minGlobal, _max, _maxGlobal) = GetMinMaxExcludedBorder(points);
            //Debug.Log(name + "\n Global (max/min): (" + _maxGlobal + " / " + _minGlobal + "}\n local (max/min): (" + String.Join(", ", _max) + " / " + String.Join(", ", _min) + " )");
            return points;
        }

        public (Vector2[] min, Vector2 minGlobal, Vector2[] max, Vector2 maxGlobal) GetMinMaxExcludedBorder(List<Vector2> points)
        {

            List<int> maxIndexList = new();
            List<int> minIndexList = new();

            for (int index = 1; index < points.Count - 1; index++)
            {
                if (points[index - 1].y < points[index].y && points[index + 1].y < points[index].y)
                    maxIndexList.Add(index - 1);
                if (points[index - 1].y > points[index].y && points[index + 1].y > points[index].y)
                    minIndexList.Add(index - 1);
            }
            List<Vector2> _maxList = new();
            List<Vector2> _minList = new();
            maxIndexList.ForEach(index => _maxList.Add(points[index]));
            minIndexList.ForEach(index => _minList.Add(points[index]));

            Vector2 _maxG = Vector2.negativeInfinity;
            Vector2 _minG = Vector2.positiveInfinity;

            _maxList.ForEach(local => _maxG = (_maxG.y < local.y) ? local : _maxG);
            _minList.ForEach(local => _minG = (_minG.y > local.y) ? local : _minG);

            Vector2? maxG = _maxG;
            Vector2? minG = _minG;
            
            if (Double.IsInfinity(_maxG.sqrMagnitude))
                maxG = null;
            if (Double.IsInfinity(_minG.sqrMagnitude))
                minG = null;
            return (_minList.ToArray(), _minG, _maxList.ToArray(), _maxG);
        }
        public Vector2 GetSignal(float position)
        {
            try
            {
                return new Vector2(position, y:function(position));
            }catch(System.DivideByZeroException e)
            {
                Debug.LogWarning(e.Message);
                return Vector2.negativeInfinity;
            }
        }
    }
}