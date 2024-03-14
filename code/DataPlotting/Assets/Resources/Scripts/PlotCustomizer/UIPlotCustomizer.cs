using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GraphPlotter.Interface;

/// <summary>
/// To Apply the settings call OnUIConfirm(). To reset call OnUIReset()
/// </summary>
public class UIPlotCustomizer : MonoBehaviour
{
    [Header("Logic")]
    [Tooltip("Links to the object holding IGraph")]
    public GameObject graphGO;

    [Header("UI")]
    [Tooltip("Links to the UI object responsible for configuring the function name.It is recommended to use the premade prefab 'Input Field' from Unity/ TextMeshPro in the UI section")]
    public TMP_InputField functionNamePlaceHolder;
    [Tooltip("Connects to the UI element dedicated to displaying the color of the plot. This feature serves a purely visual purpose, providing a representation of the plot's color without any additional functionality")]
    public Image[] uiColors;
    [Tooltip("Associates with the UI object handling the display of all active plots. Upon selection, the chosen plot becomes customizable.")]
    public TMP_Dropdown uiDropDownPlotSelector;
    [Tooltip("Links to the UI object responsible for diplaying the plot types.")]
    public TMP_Dropdown uiDropDownPlotType;

    public IGraph graph { get; private set; }
    public IPlot selectedPlot { get; private set; }
    private int lastActivePlotsAmount = -1;
    private string[] types;

    private int _currentType;
    private string _functionName;
    private Color[] _plotColors;

    private int currentType;
    private string functionName;
    private Color[] plotColors;

    private bool isEmpty = true;
    public void OnSelectPlot(int plotIndex)
    {
        SetSelectedPlot(GetPlot(plotIndex));
    }

    public void OnUIConfirm()
    {
        selectedPlot.function.name = functionName;
        selectedPlot.SetPlotType(currentType);
        selectedPlot.SetColor(plotColors);
        selectedPlot.Refresh();
        SetSelectedPlot(selectedPlot);
    }

    public void OnUIReset()
    {
        OnUIUpdateFunctionName(_functionName);
        OnUIUpdatePlotType(_currentType);
        OnUIUpdateColor(_plotColors);
        selectedPlot.Refresh();
    }

    public void OnUIUpdateColor(int type)
    {
        Debug.LogWarning("Interface for plot color is not implemented yet. Consider adding own implementation of ColorPicker and assigning the function \"OnUIUpdateColor(Color[] colors)\" to it");
    }

    public void OnUIUpdateColor(Color[] plotColors)
    {
        this.plotColors = plotColors;
        UIUpdatePlotColor(plotColors);
    }

    public void OnUIUpdateFunctionName(string functionName)
    {
        if (string.IsNullOrEmpty(functionName))
        {
            functionName = _functionName;
        }

        functionNamePlaceHolder.text = this.functionName = functionName;
    }

    public void OnUIUpdatePlotType(int currentType)
    {
        this.currentType = currentType;
        uiDropDownPlotType.SetValueWithoutNotify(currentType);
    }

    public void SetSelectedPlot(IPlot plot)
    {
        selectedPlot = plot;
        GetPlotInformationFromSelectedPlot();
        OnUIUpdateFunctionName(_functionName);
        UIUpdatePlotType(_currentType, types);
        OnUIUpdateColor(_plotColors);
    }

    internal IPlot GetPlot(int plotNumberFromActiveList)
    {
        IPlot[] plotList = graph.GetActivePlots();
        if (plotList != null && plotList.Length > 0)
            return plotList[plotNumberFromActiveList];
        return null;
    }

    internal void GetPlotInformationFromSelectedPlot()
    {
        if (selectedPlot == null)
        {
            _functionName = functionName = "";
            _currentType = currentType = 0;
            _plotColors = plotColors = new Color[] { Color.white };
            types = new string[] { "" };
            return;
        }
        _functionName = functionName = selectedPlot.function.name;
        _currentType = currentType = selectedPlot.GetPlotType();
        _plotColors = plotColors = selectedPlot.GetColor();
        if (selectedPlot is IPlot3D)
        {
            types = Enum.GetNames(typeof(ePlot3D));
        }
        if (selectedPlot is IPlot2D)
        {
            types = Enum.GetNames(typeof(ePlot2D));
        }
    }

    internal string Purify(string item)
    {
        string text = item.Trim();

        if (string.IsNullOrEmpty(text))
            return text;
        text = text[0].ToString().ToUpper() + text.Substring(1);
        return text;
    }

    private void ManageCoroutine(bool activate)
    {
        if (activate)
        {
            StartCoroutine(UpdateActivePlotList());
        }
        else
        {
            StopAllCoroutines();
        }
    }
    private void Start()
    {
        graph = graphGO.GetComponent<GraphPlotter.Interface.IGraph>();
        if (graph == null)
            Debug.LogError("For graph there is no Graph class to access. It has to be manual assigned");
        if (uiDropDownPlotSelector == null)
            Debug.LogError("For uiDropDownPlotSelector there is no TMP_Dropdown class to access. It has to be manual assigned");
        if (uiDropDownPlotType == null)
            Debug.LogError("For uiDropDown there is no TMP_Dropdown class to access. It has to be manual assigned");
        if (uiColors == null)
            Debug.LogError("For uiColors there is no UnityEngine.UI.Image class to access. It has to be manual assigned");
        if (uiColors.Length < 1)
            Debug.LogError("uiColors requires at least one element");

        ManageCoroutine(true);
        OnSelectPlot(0);
    }

    private void UIUpdatePlotColor(params Color[] colors)
    {
        for (int index = 0; index < uiColors.Length; index++)
        {
            uiColors[index].color = colors[Mathf.Clamp(index, 0, colors.Length - 1)];
        }
    }

    private void UIUpdatePlots(string[] types)
    {
        List<TMP_Dropdown.OptionData> typeList = new();
        if (types.Length < 1)
            types = new string[] { "" };
        foreach (string item in types)
        {
            string purifiedText = Purify(item);
            typeList.Add(new TMP_Dropdown.OptionData(purifiedText));
        }
        uiDropDownPlotSelector.options = typeList;
    }

    private void UIUpdatePlotType(int currentType, string[] types)
    {
        List<TMP_Dropdown.OptionData> typeList = new();
        foreach (string item in types)
        {
            string purifiedText = Purify(item);
            typeList.Add(new TMP_Dropdown.OptionData(purifiedText));
        }
        uiDropDownPlotType.options = typeList;
        uiDropDownPlotType.captionText.text = this.types[currentType];
    }

    private IEnumerator UpdateActivePlotList()
    {
        while (true)
        {
            IPlot[] plotList = graph.GetActivePlots();
            List<string> functionNames = new();
            foreach (IPlot item in plotList)
            {
                functionNames.Add(item.function.name);
            }
            lastActivePlotsAmount = uiDropDownPlotSelector.options.Count;
            UIUpdatePlots(functionNames.ToArray());
            if (isEmpty && lastActivePlotsAmount == plotList.Length)
            {
                OnSelectPlot(0);
            }
            isEmpty = (string.IsNullOrEmpty(uiDropDownPlotSelector.options[0].text) && plotList.Length == 0);
            yield return new WaitForEndOfFrame();
        }
    }
}