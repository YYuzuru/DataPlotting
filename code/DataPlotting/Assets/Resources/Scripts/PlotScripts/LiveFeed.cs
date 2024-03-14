using GraphPlotter.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LiveFeed : IFunction2D
{
    public LiveFeed(string name = null)
    {
        this.name = name;
    }
    public List<Vector2> datapoints = new();

    
    public void Update(List<Vector2> points)
    {
        datapoints.AddRange(points);
        Sort();
    }
    public void Sort()
    {
        datapoints.Sort((v1, v2) => v1.x.CompareTo(v2.x));
        datapoints.TrimExcess();
    }

    public List<Vector2> GetIntervall(float start, float end, float step, bool useLog)
    {
        if (datapoints.Count == 0)
            return new List<Vector2>();
        int smallestIndex = 0;
        int biggestIndex = Math.Clamp(0, datapoints.Count - 1, datapoints.Count - 1);
        float distanceStart = float.PositiveInfinity;
        float distanceEnd = float.PositiveInfinity;
        for (int index = 0; index < datapoints.Count; index++)
        {
            if (Mathf.Abs(start - datapoints[index].x) < distanceStart && datapoints[index].x < datapoints[smallestIndex].x)
            {
                smallestIndex = index;
                continue;
            }
            if (Mathf.Abs(end - datapoints[index].x) < distanceEnd)
            {
                biggestIndex = index;
            }
            if (datapoints[index].x > end)
                break;
        }
        return datapoints.GetRange(smallestIndex, biggestIndex - smallestIndex);
    }

    public Vector2 GetSignal(float position)
    {
        float distance = float.PositiveInfinity;
        Vector2 signal = Vector2.negativeInfinity;

        for (int index = 0; index < datapoints.Count; index++)
        {
            if (Mathf.Abs(position - datapoints[index].x) > distance)
                continue;
            distance = Mathf.Abs(position - datapoints[index].x);
            signal = datapoints[index];
        }
        return signal;
    }

    public Vector2[] max
    {
        get
        {
            return minMax.max;
        }
        private set { }
    }

    public Vector2? maxGlobal
    {
        get
        {
            return minMax.maxGlobal;
        }
        private set { }
    }

    public Vector2[] min { get {
            return minMax.min; } private set { } }

    public Vector2? minGlobal
    {
        get
        {
            return minMax.minGlobal;
        }
        private set { }
    }

    public string name { get; set; }

    (Vector2[] min, Vector2? minGlobal, Vector2[] max, Vector2? maxGlobal) minMax  =>  GetMinMaxExcludedBorder();

    (Vector2[] min, Vector2 minGlobal, Vector2[] max, Vector2 maxGlobal) GetMinMaxExcludedBorder()
    {

        List<int> maxIndexList = new();
        List<int> minIndexList = new();

        for (int index = 1; index < datapoints.Count - 1; index++)
        {
            if (datapoints[index - 1].y < datapoints[index].y && datapoints[index + 1].y < datapoints[index].y)
                maxIndexList.Add(index - 1);
            if (datapoints[index - 1].y > datapoints[index].y && datapoints[index + 1].y > datapoints[index].y)
                minIndexList.Add(index - 1);
        }
        List<Vector2> _maxList = new();
        List<Vector2> _minList = new();
        maxIndexList.ForEach(index => _maxList.Add(datapoints[index]));
        minIndexList.ForEach(index => _minList.Add(datapoints[index]));

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
}
