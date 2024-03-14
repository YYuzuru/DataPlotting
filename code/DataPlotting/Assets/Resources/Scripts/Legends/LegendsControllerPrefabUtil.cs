using GraphPlotter.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// LegendsControllerPrefabUtil is responsible for managing and displaying legend elements associated with a <see cref="IGraph"/>.
/// For full functionality <paramref name="graphGo"/>  and <paramref name="elementPrefab"/> has to be set.
/// This script does not require to be a children of <see cref="IGraph"/>
/// </summary>
//TODO: If IGraph replace with Event. Link UpdateLegend() to event on update
public class LegendsControllerPrefabUtil : MonoBehaviour
{
    [Tooltip("Reference to the GameObject representing the graph.")]
    public GameObject graphGO;
    [Tooltip("Reference to the prefab used for creating legend elements.")]
    public GameObject elementPrefab;
    [Tooltip("Boolean flag indicating whether to reset the legend elements.")]
    public bool reset = false;
    private bool _reset = false;

    List<LegendElement> elements = new();
    IGraph graph;

    private void Awake()
    {
        if (graphGO == null)
        {
            Debug.LogWarning("LegendsControllerPrefabUtil is missing link to graph. Please link graph-GameObject to this script");
        }
        graph = graphGO.GetComponent<IGraph>();
    }

    private void Start()
    {

        ManageCoroutine(true);
    }

    private void OnDisable()
    {
        ManageCoroutine(false);
    }

    private void FixedUpdate()
    {
        if(reset != _reset)
        {
            Reset();
            _reset = reset;
        } 
    }
    public void Reset()
    {
        foreach (LegendElement element in elements)
        {
            element.Remove();
        }
        elements = new();
    }

    IEnumerator UpdateLegend()
    {
        bool failure = false;
        while (!failure)
        {
            if (graph.GetActivePlots().Length < 1)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            UpdateElements(graph.GetActivePlots());
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    /// <summary>
    /// Manages the coroutine for updating the legend elements.
    /// </summary>
    /// <param name="activate">Flag to activate or deactivate the coroutine.</param>
    public void ManageCoroutine(bool activate)
    {
        if (activate)
        {
            StartCoroutine(UpdateLegend());
        }
        else
        {
            StopAllCoroutines();
        }
    }



    internal LegendElement CreateElement(string name, Color color)
    {
        GameObject prefab = Instantiate(elementPrefab);
        prefab.transform.SetParent(this.transform, false);
        LegendElement element = prefab.GetComponent<LegendElement>();
        
        element.SetColor(color);
        element.SetName(name);
        
        return element;
    }
    internal LegendElement CreateElement(IPlot plot)
    {
        GameObject prefab = Instantiate(elementPrefab);
        prefab.transform.SetParent(this.transform, false);
        LegendElement element = prefab.GetComponent<LegendElement>();

        element.Set(plot);

        return element;
    }
    internal void UpdateElements(IPlot[] plots)
    {
        for (int index = 0; index < plots.Length; index++)
        {
            if (elements.Count <= plots.Length && index >= elements.Count)
                elements.Add(CreateElement(plots[index]));
            else
                elements[index].Set(plots[index]);
        }
        if (plots.Length >= elements.Count)
            return;
        for (int index = plots.Length; index < elements.Count; index++)
        {
            elements[index].Remove();
        }
        elements.RemoveRange(plots.Length, Mathf.Abs(elements.Count - (plots.Length)));

    }

}
