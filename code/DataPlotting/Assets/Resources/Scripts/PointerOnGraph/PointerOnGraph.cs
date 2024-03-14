using GraphPlotter.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// The <see cref="PointerOnGraph"/> class is responsible for handling pointer interactions on a graph or canvas.
/// It needs to be the same child of a GameObject as the <see cref="IGraph"/> interface implementation.
/// Additionally, the GameObject containing this class requires a <see cref="BoxCollider"/> for proper functionality.
/// <see cref="PointerOnGraphGraphic"/> <paramref name="graphic"/> has to be set.
/// It is recommended to use the Prefab "PlotPicker" and set it as a child of graph which contains a canvas (For Prefab "DataPlotCanvas": Set "PlotPicker" as a child of DataPlotCanvas) or nothing will be rendered 
/// 
/// <para>
/// By invoking <see cref="PointerCapturedEvent"/>, the class will be triggered and  <see cref="ProcessPointer"/> will be executed.
/// <see cref="Invoke(UnityEngine.Ray)"/> requires a ray, which should be the device pointing location on the canvas
/// </para>
/// <para>
/// To change the content displayed. Change <see cref="DisplayData"/>
/// </para>
/// </summary>
/// 

//ToDo: split ProcessPointer into multiple functions (table and floating) information

public class PointerOnGraph : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    BoxCollider bCollider;
    IGraph graph;
    bool isOver = true;
    bool insideAxis = false;
    RectTransform rectTransform;


    public GameObject graphGameObject;

    [Tooltip("Sets color of text")]
    public Color itemColor = Color.black;

    [Tooltip("Last updated hitpoint vector of Raycast")]
    public Vector3 pointingLocation { private set; get; }

    [Tooltip("Last updated hitpoint vector of Raycast - ")]
    public Vector2 pointingOnAxisLocation { private set; get; }


    public PointerOnGraphGraphic graphic;

    [Tooltip("Contains for each active plot the location of the crosshair on the axis and the strength of ")]
    public List<(IPlot plot, Vector2 location, float signal)> datapoints { private set; get; } = new();


    bool setup = true;
    [Tooltip("Removes current data and set it back to crosshair/pointer not being set yet")]
    public bool reset = false;

    [Tooltip("displayValues will show coordinates or signalstregth of a point")]
    public bool displayValues = true;
    bool _displayValues;

    [Header("Floating Information")]
    public bool useFloating = false;
    [Tooltip("Offset in relation to Point in relation to textSize")]
    public Vector2 offsetLocal;
    public float fontSize = 20;
    private List<GameObject> infoItems = new();


    [Header("Table Information")]
    public bool useTable = true;
    private List<GameObject> tableInfoItems = new();
    public GameObject tableParent;
    public GameObject tableChildBP;

    public static event PointerCapturedEvent OnPointerCaptured;
    public delegate void PointerCapturedEvent(Ray castPoint);
    public void Invoke(Ray castPoint) => OnPointerCaptured.Invoke(castPoint);

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //isOver = false;
    }

    public void ForceDisplayUpdate()
    {
        _displayValues = !displayValues;
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (!rectTransform.localPosition.Equals(Vector3.zero) || !rectTransform.localScale.Equals(Vector3.one) || !rectTransform.localRotation.Equals(Quaternion.identity))
            Debug.LogError("Reset the location,rotation and scale of this object or utilize the parent to manipulate this gameobject");
        if (!transform.parent.parent.gameObject.TryGetComponent<IGraph>(out graph))
            Debug.LogError("This Prefab has to be the child of an IGraph");

        if (!TryGetComponent<BoxCollider>(out bCollider))
            bCollider = gameObject.AddComponent<BoxCollider>();


        rectTransform.sizeDelta = graph.rectTransform.sizeDelta;

        OnPointerCaptured += ProcessPointer;
    }


    void OnEnable()
    {
        StartCoroutine(AdjustCollision());
    }
    void OnDisable()
    {
        StopCoroutine(AdjustCollision());
    }

    IEnumerator AdjustCollision()
    {
        for (; ; )
        {
            if (bCollider == null)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            if (graph == null && graph.axis == null && !graph.axis.isActive)
            {
                bCollider.center = Vector3.zero;
                bCollider.size = Vector2.zero;

                yield return new WaitForEndOfFrame();
                continue;
            }

            try
            {
                bCollider.center = graph.axis.GetAxisCorners()[0] + (graph.axis.GetAxisCorners()[2] - graph.axis.GetAxisCorners()[0]) * 0.5f;
                bCollider.size = graph.axis.GetAxisSize();

                if (setup)
                {
                    graphic.gameObject.SetActive(true);
                    setup = false;
                }
            }
            catch (System.NullReferenceException e)
            {
                Debug.LogWarning(e.ToString());
            }

            yield return new WaitForEndOfFrame();
        }
    }



    void OnDrawGizmos()
    {
        if (graph == null)
            return;
        if (!isOver)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(graph.axis.gameObject.transform.TransformPoint(graph.axis.TranslateToWorld(pointingOnAxisLocation)), 1f);
    }

    void FixedUpdate()
    {

        if (displayValues != _displayValues)
        {
            _displayValues = displayValues;
            if (displayValues)
            {
                if (useFloating)
                {
                    DisplayInfoFloating();
                }
                if (useTable)
                {
                    DisplayInfoTable();
                }
            }
        }
        if (reset)
        {
            graphic.SetPlotData(new (IPlot plot, Vector2 location, float signal)[0]);
            RemoveDisplayInfoFloating();
            RemoveDisplayInfoTable();

            reset = false;
        }
    }

    private void ProcessPointer(Ray castPoint)
    {
        RaycastHit hit;
        insideAxis = Physics.Raycast(castPoint, out hit, Mathf.Infinity);
        if (insideAxis)
        {
            pointingLocation = hit.point;
            pointingOnAxisLocation = graph.axis.TranslateToAxis(graph.axis.gameObject.transform.InverseTransformPoint(pointingLocation));
            datapoints = GetPoints();
            graphic.SetPlotData(datapoints.ToArray());

            if (displayValues)
                ForceDisplayUpdate();
        }

    }

    private string DisplayData((IPlot plot, Vector2 location, float signal) plotPoint)
    {
        string content = plotPoint.location.x + " -> " + plotPoint.signal;

        if (plotPoint.plot is IPlot3D)
            content = plotPoint.location + " -> " + plotPoint.signal;

        return content;
    }


    internal Vector3 GetPointOfPlot(IPlot plot, Vector2 point)
    {
        IPlot2D plot2D;
        IPlot3D plot3D;

        if (plot.gameObject.TryGetComponent<IPlot2D>(out plot2D))
        {
            Vector3 signal = plot2D.GetSignal(point.x);
            signal.z = signal.y;
            return signal;
        }
        if (plot.gameObject.TryGetComponent<IPlot3D>(out plot3D))
        {
            return plot3D.GetSignal(point);
        }
        throw new System.NotImplementedException();
    }
    internal List<(IPlot plot, Vector2 location, float signal)> GetPoints()
    {

        if (!isOver)
            return new();

        Vector2 nPoint = graph.axis.TranslateToAxis(graph.axis.gameObject.transform.InverseTransformPoint(pointingLocation));
        datapoints.Clear();
        graph.GetActivePlots().ToList().ForEach(
            plot =>
            {
                Vector3 dataSignal = GetPointOfPlot(plot, nPoint);
                if (InsideAxis(dataSignal, plot))
                    datapoints.Add((plot, dataSignal, dataSignal.z));
            });
        return datapoints;


        bool InsideAxis(Vector2 location, IPlot plot)
        {
            IAxis axis = plot.axis;

            if (plot is IPlot3D && (((IAxis2D)axis).useYLog || ((IAxis2D)axis).useXLog))
                return false;
            //Debug.Log(location +" in range" + axis.GetYAxisIntervall()[0]+" -> "+ axis.GetYAxisIntervall()[1]);
            if (((IAxis2D)axis).useYLog && (location.y <= Mathf.Pow(10, axis.GetYAxisIntervall()[0]) || location.y >= Mathf.Pow(10, axis.GetYAxisIntervall()[1])))
                return false;
            if (!((IAxis2D)axis).useYLog && (location.y < axis.GetYAxisIntervall()[0] || location.y > axis.GetYAxisIntervall()[1]))
                return false;
            return true;
        }
    }


    internal void DisplayInfoFloating()
    {
        RemoveDisplayInfoFloating();
        foreach ((IPlot plot, Vector2 location, float signal) element in datapoints)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(graphic.transform, false);
            RectTransform rcT = go.AddComponent<RectTransform>();
            TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();

            ContentSizeFitter fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            infoItems.Add(go);

            Vector2 location = Vector2.zero;
            Vector2 offset = rcT.sizeDelta * offsetLocal;
            go.transform.localPosition = location + offset;

            text.fontSize = fontSize;
            text.text = DisplayData(element);
            text.color = itemColor;
        }

    }
    internal void RemoveDisplayInfoFloating()
    {
        foreach (GameObject go in infoItems)
        {
            GameObject.Destroy(go);
        }
    }

    internal void DisplayInfoTable()
    {
        RemoveDisplayInfoTable();

        datapoints.ForEach(datapoint =>
        {
            string content = DisplayData(datapoint);
            tableInfoItems.Add(CreateTableElement(content, itemColor));
        });

    }
    internal void RemoveDisplayInfoTable()
    {
        if (!CheckTable()) return;
        tableInfoItems.ForEach(item => Destroy(item));
        tableInfoItems = new();
    }

    internal bool CheckTable()
    {
        return (tableParent != null) && (tableChildBP != null);
    }
    internal GameObject CreateTableElement(string content, Color color)
    {
        GameObject item = Instantiate(tableChildBP);
        item.transform.SetParent(tableParent.transform, false);
        LegendElement itemAccess = item.GetComponent<LegendElement>();
        itemAccess.SetName(content);
        itemAccess.SetColor(color);

        return item;
    }
}