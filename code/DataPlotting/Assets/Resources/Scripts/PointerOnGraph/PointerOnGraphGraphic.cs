using GraphPlotter.Interface;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for displaying the crosshair on the individual axis. To update or Init the pointer, set <see cref="SetPlotData"/>
/// </summary>
public class PointerOnGraphGraphic : MaskableGraphic
{
    public float thickness = 5;
    public float size = 9;
    public Color itemColor = Color.black;

    (IPlot plot, Vector2 location, float signal)[] plotData = new (IPlot plot, Vector2 location, float signal)[0];
    Vector3 wireSphereLocation = Vector3.zero;
    RectTransform rctT = null;


    [Tooltip("Stores location of the pointer on the axis for the plot. In case of IPlot3D signal is used to store the z value. For IPlot2D signal is location.y")]
    public void SetPlotData((IPlot plot, Vector2 location, float signal)[] nPlotData)
    {
        plotData = nPlotData;
        SetVerticesDirty();
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        rctT = rctT ?? transform.GetComponent<RectTransform>();
        vh.Clear();
        UIVertex vertex = UIVertex.simpleVert;
        foreach ((IPlot plot, Vector2 location, float signal) element in plotData)
        {
            vertex.color = itemColor;
            Vector2 location = element.plot.axis.TranslateToWorld(element.location); /*element.plot.axis.gameObject.transform.TransformPoint(element.plot.axis.TranslateToWorld(element.location));*/
            Vector2 offset = .5f * rctT.sizeDelta;
            wireSphereLocation = gameObject.transform.TransformPoint(location - offset);
            DrawPoint((location) - offset, vertex, vh);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wireSphereLocation, 3);
    }

    void DrawPoint(Vector2 position, UIVertex vertex, VertexHelper vh)
    {
        UIVertex[] vert = CopyVertex(vertex, 4);
        Vector2 halfSize = Vector2.one * (thickness * .5f);

        //xAxis
        vert[0].position = new(x: position.x - size, y: position.y - halfSize.y, z: 0);
        vert[3].position = new(x: position.x + size, y: position.y - halfSize.y, z: 0);
        vert[1].position = new(x: position.x - size, y: position.y + halfSize.y, z: 0);
        vert[2].position = new(x: position.x + size, y: position.y + halfSize.y, z: 0);
        vh.AddUIVertexQuad(vert);

        //yAxis
        vert[0].position = new(y: position.y - size, x: position.x - halfSize.x, z: 0);
        vert[3].position = new(y: position.y + size, x: position.x - halfSize.x, z: 0);
        vert[1].position = new(y: position.y - size, x: position.x + halfSize.x, z: 0);
        vert[2].position = new(y: position.y + size, x: position.x + halfSize.x, z: 0);
        vh.AddUIVertexQuad(vert);
    }

    UIVertex[] CopyVertex(UIVertex src, int amount)
    {
        UIVertex[] res = new UIVertex[amount];
        for (int index = 0; index < amount; index++)
        {
            UIVertex vert = UIVertex.simpleVert;
            vert.position = src.position;
            vert.color = src.color;
            res[index] = vert;
        }
        return res;
    }
}