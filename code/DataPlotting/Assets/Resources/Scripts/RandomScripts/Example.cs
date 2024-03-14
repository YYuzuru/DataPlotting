using GraphPlotter;
using GraphPlotter.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Plot
{
    public Plot(IPlot iplot)
    {
        this.plot = iplot;
        name = iplot.gameObject.name;
        color = iplot.GetColor();
        display = false;
        type = "";
        thickness = 1f;
        pointDensity = 1;
        changeType = 0;
        UpdateType();
    }
    public string name;
    public bool display;
    public IPlot plot;
    [Tooltip("Multple Colors are not supported on all plotTypes")]
    public Color[] color;
    public int changeType;
    public string type;

    [Tooltip("Thickness and pointDensity is not supported on all plotTypes")]
    public float thickness;
    public float pointDensity;

    public void UpdateType()
    {
        int iType = plot.GetPlotType();
        IPlot2D plot2D;
        IPlot3D plot3D;
        if(plot.gameObject.TryGetComponent<IPlot2D>(out plot2D))
        {
            type = plot2D.plotType.ToString();
            return;
        }
        if(plot.gameObject.TryGetComponent<IPlot3D>(out plot3D))
        {
            type = plot3D.plotType.ToString();
            return;
        }
    }

    public void UpdateParametes()
    {
        IPlot2D plot2d = plot.gameObject.GetComponent<IPlot2D>();
        if(plot2d != null)
        {
            plot2d.SetLineStrength(thickness);
            plot2d.SetPointDensity(pointDensity);
        }
    }
    public void Refresh()
    {
        plot.SetColor(color);
        plot.SetPlotType(changeType);
        plot.Refresh();
        UpdateParametes();
        UpdateType();
    }

}



public class Example : MonoBehaviour
{
    public GameObject Graph;
    public IGraph2D graph2D;
    public IGraph3D graph3D;
    public List<Plot> plotList = new();
    public List<CSV> csvList = new();
    List<Vector2> cpf = new();
    LiveFeed feed;
    LiveFeed log;

    float time;

    private void Awake()
    {
        plotList = new();
        graph2D = Graph.GetComponent<IGraph2D>();
        graph3D = Graph.GetComponent<IGraph3D>();
        feed = new LiveFeed();
        feed.name = "Feed";

        log = new LiveFeed();
        log.name = "log";
        log.datapoints = new() { Vector2.one * Mathf.Pow(10, 0), Vector2.one * Mathf.Pow(10, 1)/*, Vector2.one * Mathf.Pow(10, 2)*/ };
    }

    void Start()
    {
        SetExamples();
        StartCoroutine(UpdateFeed());
        StartCoroutine(UpdateCos());
    }

    private void Update()
    {
        time += Time.deltaTime;

    }
    private void FixedUpdate()
    {

        for (int index = 0; index < plotList.Count; index++)
        {
            Plot plot = plotList[index];
            if (plot.plot.gameObject.activeSelf != plot.display)
            {
                plot.Refresh();

                plot.plot.gameObject.SetActive(plot.display);
            }
            if (plot.display)
            {
                plot.display = false;
            }
        }
    }
    void SetExamples()
    {


        AddPlot2D(new Function(x => Mathf.Log10(x), "log10-Funktion"), new Color[] { Color.black });
        AddPlot2D(new Function(x => Mathf.Exp(x), "Exp Funktion"), new Color[] { Color.black });
        AddPlot2D(new Function(x => 1 / x, "gebrochen-rationale Funktion"), new Color[] { Color.black });
        AddPlot2D(new Function(x => Mathf.Sin(x) * 6, "sinus-funktion"), new Color[] { Color.black });
        AddPlot2D(new Function(x => (int)(x), "Integer-funktion"), new Color[] { Color.black });
        AddPlot2D(new Function(x => (x), "linear-funktion"), new Color[] { Color.black });
        AddPlot2D(feed, new Color[] { Color.black });
        AddPlot2D(log, new Color[] { Color.black });
        if (csvList.Count > 0)
            AddPlot3D(new FunctionHeatMap(DataGenerator.CompileCSV(new(csvList[0].xValuesFilePath, csvList[0].yValuesFilePath, csvList[0].signalValuesFilePath)), "HeatMap"), new Color[] { Color.blue, Color.red });
    }

    public Plot AddPlot2D(IFunction2D function, Color[] color)
    {
        Plot plot = new(graph2D.AddPlot(function, color));
            plotList.Add(plot);
        return plot;
    }
    public Plot AddPlot3D(IFunction3D function, Color[] color)
    {
        Plot plot = new(graph3D.AddPlot(function, color));
        plotList.Add(plot);
        return plot;
    }

    private IEnumerator UpdateFeed()
    {
        for (; ; )
        {
            feed.Update(cpf);
            if (cpf.Count < 1)
                yield return new WaitForSeconds(1f);
            cpf.Clear();
            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator UpdateCos()
    {
        for (; ; )
        {
            cpf.Add(new Vector2(x: time, y: Mathf.Cos(time) * 6));
            yield return new WaitForSeconds(.5f);
        }
    }


}
