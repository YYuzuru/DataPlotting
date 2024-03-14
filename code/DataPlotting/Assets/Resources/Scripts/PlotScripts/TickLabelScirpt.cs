using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperScirpts;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.UIElements;

namespace GraphPlotter
{
    public class TickLabelScirpt : MonoBehaviour
    {
        public ControllerScript controllerScript;
        public int fontSize;
        public Color color;
        public float offset;
        private AxisScript axis;
        private Vector2[] xTickPositions;
        private Vector2[] yTickPositions;
        private float xSpaceBetweenTicks;
        private float ySpaceBetweenTicks;
        private float xStartValue;
        private float yStartValue;
        private float xStepSize;
        private float yStepSize;

        private GameObject xLabelContainer;
        private GameObject yLabelContainer;
        private string[] xLabels;
        private string[] yLabels;

        private List<GameObject> xLabelsList;
        private List<GameObject> yLabelsList;

        int xStepSizeScaler = 1;
        int yStepSizeScaler = 1;

        public void RedrawTickLabels()
        {
            axis = this.transform.GetComponentInParent<AxisScript>();
            xTickPositions = axis.xTickPositions;
            yTickPositions = axis.yTickPositions;
            xSpaceBetweenTicks = axis.xSpaceBetweenTicks;
            ySpaceBetweenTicks = axis.ySpaceBetweenTicks;
            xStartValue = axis.xLeftBoundary;
            yStartValue = axis.yLeftBoundary;
            xStepSize = axis.xStepSize;
            yStepSize = axis.yStepSize;

            if (xLabelContainer != null) Destroy(xLabelContainer);
            if (yLabelContainer != null) Destroy(yLabelContainer);

            xLabelContainer = new GameObject("xLabelContainer");
            xLabelContainer.transform.SetParent(this.transform,false);
            xLabelContainer.transform.localPosition = this.transform.localPosition;
            xLabelContainer.transform.localRotation = this.transform.localRotation;
            xLabelContainer.transform.localScale = Vector3.one;

            yLabelContainer = new GameObject("yLabelContainer");
            yLabelContainer.transform.SetParent(this.transform, false);
            yLabelContainer.transform.localPosition = this.transform.localPosition;
            yLabelContainer.transform.localRotation = this.transform.localRotation;
            yLabelContainer.transform.localScale = Vector3.one;

            SpawnTickLabels();
            ResizeInputFields();
        }

        private void SpawnTickLabels()
        {
            GenerateStringArrays();

            xLabelsList = new List<GameObject>();
            yLabelsList = new List<GameObject>();

            for (int i = 0; i < xLabels.Length; i += xStepSizeScaler)
            {
                string labelName = xLabels[i];
                GameObject textObject = new GameObject(labelName, typeof(TextMeshProUGUI), typeof(ContentSizeFitter));
                textObject.transform.SetParent(xLabelContainer.transform, false);
                textObject.transform.localRotation = Quaternion.identity;
                textObject.transform.localScale = Vector3.one;
                TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
                text.text = labelName;
                text.color = Color.black;
                text.fontSize = fontSize;

                RectTransform textRect = textObject.GetComponent<RectTransform>();
                textRect.anchoredPosition3D = new Vector3(xTickPositions[i].x, xTickPositions[i].y - offset);
                textRect.pivot = new Vector2(0.5f, 1.0f);
                text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                text.verticalAlignment = VerticalAlignmentOptions.Top;
                textObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                textObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                xLabelsList.Add(textObject);
            }

            for (int i = 0; i < yLabels.Length; i += yStepSizeScaler)
            {
                string labelName = yLabels[i];
                GameObject textObject = new GameObject(labelName, typeof(TextMeshProUGUI), typeof(ContentSizeFitter));
                textObject.transform.SetParent(yLabelContainer.transform, false);
                textObject.transform.localRotation = Quaternion.identity;
                textObject.transform.localScale = Vector3.one;
                TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
                text.text = labelName;
                text.color = Color.black;
                text.fontSize = fontSize;

                RectTransform textRect = textObject.GetComponent<RectTransform>();
                textObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                textObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                textRect.anchoredPosition3D = new Vector3(yTickPositions[i].x - offset, yTickPositions[i].y);
                textRect.pivot = new Vector2(1.0f, 0.5f);
                text.horizontalAlignment = HorizontalAlignmentOptions.Right;
                text.verticalAlignment = VerticalAlignmentOptions.Middle;

                yLabelsList.Add(textObject);
            }

            StartCoroutine(FindStepSizeScaler());
        }

        private void GenerateStringArrays()
        {
            if (controllerScript == null)
                Debug.LogError("controllerScript not set");

            xLabels = new string[xTickPositions.Length];
            int xDecimals = controllerScript.xMaxDecimalAmount;
            for (int i = 0; i < xLabels.Length; i++)
            {
                float val = xStartValue + i * xStepSize;
                xLabels[i] = TextEditing.CorrectDeciamlsAmount(val.ToString(), xDecimals);
            }

            yLabels = new string[yTickPositions.Length];
            int yDecimals = controllerScript.yMaxDecimalAmount;
            for (int i = 0; i < yLabels.Length; i++)
            {
                float val = yStartValue + i * yStepSize;
                yLabels[i] = TextEditing.CorrectDeciamlsAmount(val.ToString(), yDecimals);
            }

        }

        public void ResizeInputFields()
        {
            TextEditing.ChangeFontSize("xLeftValue", fontSize);
            TextEditing.ChangeFontSize("xRightValue", fontSize);
            TextEditing.ChangeFontSize("yLeftValue", fontSize);
            TextEditing.ChangeFontSize("yRightValue", fontSize);
            TextEditing.ChangeFontSize("xStepSize", fontSize);
            TextEditing.ChangeFontSize("yStepSize", fontSize);

            Vector2 deltaSize = new Vector2(fontSize * 3, fontSize * 2);
            TextEditing.ChangeDeltaSize("xLeftValue", deltaSize);
            TextEditing.ChangeDeltaSize("xRightValue", deltaSize);
            TextEditing.ChangeDeltaSize("yLeftValue", deltaSize);
            TextEditing.ChangeDeltaSize("yRightValue", deltaSize);
            TextEditing.ChangeDeltaSize("xStepSize", deltaSize);
            TextEditing.ChangeDeltaSize("yStepSize", deltaSize);

            Vector2 toggleSize = new Vector2(deltaSize.y, deltaSize.y);
            TextEditing.ChangeDeltaSizeToggle("xLogToggle", toggleSize);
            TextEditing.ChangeDeltaSizeToggle("yLogToggle", toggleSize);

            if (controllerScript == null)
                Debug.LogError("controllerScript not set");
            float canvasWidth = controllerScript.graph.rectTransform.rect.width;
            float canvasHeight = controllerScript.graph.rectTransform.rect.height;
            Vector2 bottomLeft = new Vector2(-canvasWidth / 2, -canvasHeight / 2);

            /*
            Vector3 xLeftInputPos = new Vector2(bottomLeft.x + axis.xCalculatedStartPosition.x, bottomLeft.y + 2 * deltaSize.y);
            Vector3 xRightInputPos = new Vector2(bottomLeft.x + axis.xCalculatedEndPosition.x, bottomLeft.y + 2 * deltaSize.y);
            
            Vector3 yLeftInputPos = new Vector2(bottomLeft.x + axis.xCalculatedStartPosition.x, bottomLeft.y + deltaSize.y);
            Vector3 yRightInputPos = new Vector2(bottomLeft.x + axis.xCalculatedEndPosition.x, bottomLeft.y + deltaSize.y);
            
            Vector3 xStepSizeInputPos = new Vector2((xLeftInputPos.x + xRightInputPos.x) / 2, xLeftInputPos.y);
            Vector3 yStepSizeInputPos = new Vector2((yLeftInputPos.x + yRightInputPos.x) / 2, yLeftInputPos.y);
            */

            float xStartPos = bottomLeft.x + axis.xCalculatedStartPosition.x;

            Vector3 xLeftInputPos = new Vector2(xStartPos, bottomLeft.y + 2 * deltaSize.y);
            Vector3 yLeftInputPos = new Vector2(xStartPos, bottomLeft.y + deltaSize.y);

            Vector3 xStepSizeInputPos = new Vector2(xStartPos + axis.xCalculatedLength / 3, bottomLeft.y + 2 * deltaSize.y);
            Vector3 yStepSizeInputPos = new Vector2(xStartPos + axis.xCalculatedLength / 3, bottomLeft.y + deltaSize.y);

            Vector3 xRightInputPos = new Vector2(xStartPos + (axis.xCalculatedLength / 3) * 2, bottomLeft.y + 2 * deltaSize.y);
            Vector3 yRightInputPos = new Vector2(xStartPos + (axis.xCalculatedLength / 3) * 2, bottomLeft.y + deltaSize.y);

            Vector3 xLogTogglePos = new Vector2(xStartPos + axis.xCalculatedLength, bottomLeft.y + 2 * deltaSize.y);
            Vector3 yLogTogglePos = new Vector2(xStartPos + axis.xCalculatedLength, bottomLeft.y + deltaSize.y);

            TextEditing.ChangeAchoredPosition3D("xLeftValue", xLeftInputPos);
            TextEditing.ChangeAchoredPosition3D("xRightValue", xRightInputPos);
            TextEditing.ChangeAchoredPosition3D("yLeftValue", yLeftInputPos);
            TextEditing.ChangeAchoredPosition3D("yRightValue", yRightInputPos);
            TextEditing.ChangeAchoredPosition3D("xStepSize", xStepSizeInputPos);
            TextEditing.ChangeAchoredPosition3D("yStepSize", yStepSizeInputPos);
            TextEditing.ChangeAchoredPosition3D("xLogToggle", xLogTogglePos);
            TextEditing.ChangeAchoredPosition3D("yLogToggle", yLogTogglePos);
        }

        IEnumerator FindStepSizeScaler()
        {
            yield return new WaitForEndOfFrame();

            float maxWidth = axis.xCalculatedLength;
            float maxHeight = axis.yCalculatedLength;
            float currentWidth = 0f;
            float currentHeight = 0f;
            bool redrawX = false;
            bool redrawY = false;

            foreach (GameObject go in xLabelsList)
            {
                currentWidth += go.GetComponent<RectTransform>().rect.width;
                if (isIntersectingAxis(go.GetComponent<RectTransform>())) go.SetActive(false);
            }

            foreach (GameObject go in yLabelsList)
            {
                currentHeight += go.GetComponent<RectTransform>().rect.height;
                if (isIntersectingAxis(go.GetComponent<RectTransform>())) go.SetActive(false);
            }

            if (2 * currentWidth > maxWidth)
            {
                redrawX = true;
                xStepSizeScaler++;
            }
            if (2 * currentHeight > maxHeight)
            {
                redrawY = true;
                yStepSizeScaler++;
            }

            if (redrawX || redrawY)
            {
                RedrawTickLabels();
            }
            else
            {
                xStepSizeScaler = 1;
                yStepSizeScaler = 1;
            }
        }

        private bool isIntersectingAxis(RectTransform rect)
        {
            Vector2 axisOrigin = axis.intersectionPosition;
            float offset = axis.lineStrength;
            Vector2 originalPivot = rect.pivot;
            rect.pivot = new Vector2(0.5f, 0.5f);
            Vector2 pos = rect.anchoredPosition;
            Vector2 size = rect.sizeDelta;
            bool result = false;

            if (pos.x >= axisOrigin.x - offset && pos.x <= axisOrigin.x + offset)
            {
                if (pos.y <= axis.yDrawStartPosition.y || pos.y >= axis.yDrawEndPosition.y) result = false;
                else result = true;
            }
            if (pos.y >= axisOrigin.y - offset && pos.y <= axisOrigin.y + offset)
            {
                if (pos.x <= axis.xDrawStartPosition.x || pos.x >= axis.xDrawEndPosition.x) result = false;
                else result = true;
            }
            rect.pivot = originalPivot;
            return result;
        }
    }
}
