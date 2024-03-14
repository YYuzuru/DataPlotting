using GraphPlotter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DataGenerator;

public class FunctionHeatMap : IFunction3D
{

    public FunctionHeatMap(DataPoint[,] function, string name  = null)
    {
        this.name = name ?? "Heatmap "+ this.GetHashCode();
        this.function = function;
    }
    public DataPoint[,] function;

    public Vector3[] max => throw new NotImplementedException();

    public Vector3? maxGlobal => throw new NotImplementedException();

    public Vector3[] min => throw new NotImplementedException();

    public Vector3? minGlobal => throw new NotImplementedException();

    public string name { get; set; }

    public DataPoint[,] GetIntervall()
    {
        return function;
    }

    public Vector3 GetSignal(Vector2 position)
    {
        float minDistance = float.MaxValue;
        Vector3 signal = Vector3.negativeInfinity;
        for (int indexY = 0; indexY < function.GetLength(0); indexY++)
        {
            float distance = float.MaxValue;
            for (int indexX = 0; indexX < function.GetLength(1); indexX++)
            {
                Vector2 currentPos = new(x: function[indexY, indexX].Get.x, y: function[indexY, indexX].Get.y);
                float currentDistance = Vector2.Distance(currentPos, position);
                if (distance - currentDistance < 0)
                    break;
                distance = currentDistance;
                if (distance < minDistance)
                {
                    signal = function[indexY, indexX].Get;
                    minDistance = distance;
                }
            }

        }
        return signal;
    }

    (Vector3[] min, Vector3? minGlobal, Vector3[] max, Vector3? maxGlobal) minMax => GetMinMaxExcludedBorder();

    (Vector3[] min, Vector3? minGlobal, Vector3[] max, Vector3? maxGlobal) GetMinMaxExcludedBorder()
    {

        List<Vector2Int> maxIndexList = new();
        List<Vector2Int> minIndexList = new();
        List<Vector3> _maxList = new();
        List<Vector3> _minList = new();

        if (function.GetLength(0) * function.GetLength(1) < 3)
            return (_minList.ToArray(), null, _maxList.ToArray(), null);
        for(int indexY =  1; indexY < function.GetLength(0)-1; indexY++)
        {
            for (int indexX = 1; indexX < function.GetLength(1) - 1; indexX++)
            {
                Vector3[] kernel = new Vector3[] {
                    function[indexY - 1, indexX - 1].Get, function[indexY - 1, indexX].Get, function[indexY - 1, indexX + 1].Get,
                    function[indexY, indexX - 1].Get, function[indexY, indexX].Get, function[indexY, indexX+1].Get,
                    function[indexY+1, indexX - 1].Get, function[indexY+1, indexX].Get, function[indexY+1, indexX+1].Get,
                };
                kernel = kernel.ToList().OrderBy(point => point.z).ToArray().ToArray();
                if (kernel[0].x == function[indexY, indexX].x && kernel[0].y == function[indexY, indexX].y)
                    minIndexList.Add(new(x: indexX, y: indexY));
                else if (kernel[^1].x == function[indexY, indexX].x && kernel[^1].y == function[indexY, indexX].y)
                    maxIndexList.Add(new(x: indexX, y: indexY));
            }
        }
        maxIndexList.ForEach(index => _maxList.Add(function[index.y,index.x].Get));
        minIndexList.ForEach(index => _minList.Add(function[index.y,index.x].Get));
        Vector3 _maxG = Vector2.negativeInfinity;
        Vector3 _minG = Vector2.positiveInfinity;

        _maxList.ForEach(local => _maxG = (_maxG.z < local.z) ? local : _maxG);
        _minList.ForEach(local => _minG = (_minG.z > local.z) ? local : _minG);
        
        Vector3? maxG = _maxG;
        Vector3? minG = _minG;

        if (Double.IsInfinity(_maxG.sqrMagnitude))
            maxG = null;
        if (Double.IsInfinity(_minG.sqrMagnitude))
            minG = null;
        return (_minList.ToArray(), minG, _maxList.ToArray(), maxG);
    }
}