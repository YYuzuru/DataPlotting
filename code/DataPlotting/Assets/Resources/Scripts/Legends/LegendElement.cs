using GraphPlotter.Interface;
using System;
using TMPro;
using UnityEngine;

/// <summary>
/// LegendElement is a MonoBehaviour representing an individual plot of the graph.
/// </summary>
public class LegendElement : MonoBehaviour
{
    public TextMeshProUGUI text { protected set; get; }
    void Awake()
    {
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
    }
    /// <summary>
    /// Sets the name, text, and gameObject name for the legend element.
    /// </summary>
    /// <param name="name">Name and displayed text to be set for the legend element.</param>
    public void SetName(string name)
    {
        gameObject.name = name;
        text.name = name;
        text.text = name;
    }
    /// <summary>
    /// Sets the color for the legend element's text.
    /// </summary>
    /// <param name="color">Color to be set for the text.</param>
    public void SetColor(Color color)
    {
        text.color = color;
    }

    /// <summary>
    /// Sets the name and color for the legend element based on the provided <see cref="IPlot"/>.
    /// </summary>
    /// <param name="plot">Reference to the IPlot interface.</param>
    public void Set(IPlot plot)
    {
        SetName(plot.function.name);
        SetColor(plot.GetColor()[0]);
    }
    public string GetName()
    {
        return text.text;
    }
    public Color GetColor()
    {
        return text.color;
    }

    /// <summary>
    /// Removes the legend element by destroying the associated GameObject and the script instance.
    /// </summary>
    public void Remove()
    {
        GameObject.Destroy(gameObject);
        GameObject.Destroy(this);
    }
}
