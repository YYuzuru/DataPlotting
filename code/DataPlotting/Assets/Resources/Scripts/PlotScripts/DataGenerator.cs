using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

[System.Serializable]
public struct CSV
{
    public CSV(string xValuesPath, string yValuesPath, string signalValuesPath)
    {
        xValuesFilePath = xValuesPath;
        yValuesFilePath = yValuesPath;
        signalValuesFilePath = signalValuesPath;
    }
    public string xValuesFilePath;
    public string yValuesFilePath;
    public string signalValuesFilePath;
}


public class DataGenerator : MonoBehaviour
{
    public struct DataPoint
    {
        public DataPoint(Vector2 point, float signal)
        {
            this.x = point.x;
            this.y = point.y;
            this.signal = signal;
        }
        public float x;
        public float y;
        public float signal;

        public Vector3 Get => new Vector3(x, y, signal);
    }

    public string xValuesFilePath;
    public string yValuesFilePath;
    public string signalsFilePath;

    string[] xValuesExtracedStringArray;
    string[] yValuesExtracedStringArray;
    string[][] signalValuesExtractedStringArray;

    public DataPoint[,] dataPointsArray2D { get; set; }

    public void Start()
    {
        string[] xValuesTemp = File.ReadAllLines(xValuesFilePath);
        xValuesExtracedStringArray = xValuesTemp[0].Split(',', ';');

        string[] yValuesTemp = File.ReadAllLines(yValuesFilePath);
        yValuesExtracedStringArray = yValuesTemp[0].Split(',', ';');

        string[] signalTemp = File.ReadAllLines(signalsFilePath);
        signalValuesExtractedStringArray = new string[signalTemp.Length][];
        for (int i = 0; i < signalTemp.Length; i++)
        {
            signalValuesExtractedStringArray[i] = signalTemp[i].Split(',', ';');
        }

        dataPointsArray2D = new DataPoint[xValuesExtracedStringArray.Length, yValuesExtracedStringArray.Length];

        for (int i = 0; i < xValuesExtracedStringArray.Length; i++)
        {
            for (int j = 0; j < yValuesExtracedStringArray.Length; j++)
            {
                DataPoint dataPoint = new DataPoint();
                dataPoint.x = float.Parse(xValuesExtracedStringArray[i], CultureInfo.InvariantCulture);
                dataPoint.y = float.Parse(yValuesExtracedStringArray[j], CultureInfo.InvariantCulture);
                dataPoint.signal = float.Parse(signalValuesExtractedStringArray[i][j], CultureInfo.InvariantCulture);
                dataPointsArray2D[i, j] = dataPoint;
            }
        }
    }

    public static DataPoint[,] CompileCSV(CSV path, params char[] splitter)
    {
        splitter = (splitter.Length>0)?splitter ?? new[] { ',', ';' }: new[] { ',', ';' };
        string[] xVal = File.ReadAllLines(path.xValuesFilePath)[0].Split(splitter);
        string[] yVal = File.ReadAllLines(path.yValuesFilePath)[0].Split(splitter);

        string[] signalTemp = File.ReadAllLines(path.signalValuesFilePath);
        string[][] signalVal = new string[signalTemp.Length][];
        for (int i = 0; i < signalTemp.Length; i++)
        {
            signalVal[i] = signalTemp[i].Split(',', ';');
        }

        //Debug.Log(string.Join(" | ",splitter)+"\n"+string.Join(" | ", xVal));

        DataPoint[,] dataPointsDictionary = new DataPoint[xVal.Length,yVal.Length];
        for(int indexY = 0;indexY < yVal.Length; indexY++)
        {
            for (int indexX = 0; indexX < xVal.Length; indexX++)
            {
                Vector2 dataPoint = new Vector3();
                dataPoint.x = float.Parse(xVal[indexX], CultureInfo.InvariantCulture);
                dataPoint.y = float.Parse(yVal[indexY], CultureInfo.InvariantCulture);
                float signal = 0;
                try
                {
                    signal = float.Parse(signalVal[indexX][indexY], CultureInfo.InvariantCulture);
                }
                catch (ArgumentOutOfRangeException) {
                    Debug.LogWarning("(current|max) Index X: " + indexX + " | " + xVal.Length+ "\n" + "(current|max) Index Y: " + indexY + " | " + yVal.Length);
                }
                dataPointsDictionary[indexX,indexY] = new (dataPoint, signal);
            }
        }
        return dataPointsDictionary;
    }


    private static string[] ReadCSV(string path, params char[] splitter)
    {
        string[] file = File.ReadAllLines(path);
        string[] values = file[0].Split(splitter);

        return values;
    }


}
