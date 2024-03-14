using UnityEngine;

public class PointerOnGraphMouseInput : MonoBehaviour
{
    public bool useMouse = true;
    public PointerOnGraph pointerOnGraph;

    void Update()
    {
        if (useMouse && (Input.GetMouseButtonDown(0)))
        {
            Vector3 mouse = Input.mousePosition;
            pointerOnGraph.Invoke(Camera.main.ScreenPointToRay(mouse));
        }
    }
}
