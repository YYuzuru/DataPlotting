using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    public PointerOnGraph graphProcessor;
    public bool Debug;

    Ray mouseRay;
    Vector3 mouseLocation;
    bool mousePressed;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseLocation = Input.mousePosition;
            mouseRay = Camera.main.ScreenPointToRay(mouseLocation);
            graphProcessor.Invoke(mouseRay);
            mousePressed = true;

        }
    }
    private void OnDrawGizmos()
    {
        if (Debug && mousePressed)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(mouseLocation, graphProcessor.pointingLocation);

        }
    }
}
