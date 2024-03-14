using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperScirpts;
using UnityEngine.UI;
using TMPro;
using GraphPlotter.Interface;
using System.Runtime.CompilerServices;
using System;

namespace GraphPlotter
{
    public class ControllerScript : MonoBehaviour, IAxis2D, IAxis3D
    {
        public bool updateMask;
        public GameObject axis;
        public AxisScript axisScript;



        //Childrens
        private RectMask2D mask;
        public float xLeftValue { private set; get; }
        public float xStepSize { private set; get; }
        public float xRightValue { private set; get; }
        public int xMaxDecimalAmount { private set; get; }

        public float yLeftValue { private set; get; }
        public float yStepSize { private set; get; }
        public float yRightValue { private set; get; }
        public int yMaxDecimalAmount { private set; get; }

        public bool UseXLog { private set; get; }
        public int xExpStart { private set; get; }
        public int xExpEnd { private set; get; }
        public bool UseYLog { private set; get; }
        public int yExpStart { private set; get; }
        public int yExpEnd { private set; get; }


        [Header("UsePreset")]
        public bool useDefaultSettings;
        public float xDefaultStepSize = 1;
        public float yDefaultStepSize = 1;
        [Header("Linear Axis")]
        public float xDefaultLeftValue = -10;
        public float xDefaultRightValue = 10;
        public int xDefaultMaxDecimalAmount = 1;

        public float yDefaultLeftValue = -10;
        public float yDefaultRightValue = 10;
        public int yDefaultMaxDecimalAmount = 1;

        [Header("Logarithmic Axis")]

        public bool xDefaultUseLog = false;
        public int xDefaultExpStart = 0;
        public int xDefaultExpEnd = 1;
        public bool yDefaultUseLog = false;
        public int yDefaultExpStart = 0;
        public int yDefaultExpEnd = 1;

        private bool inputValuesChanged;
        private bool initalValuesAllSet;
        public bool isActive => _isActive;
        bool _isActive = false;


        private static float constantLog = (1 / Mathf.Log10(10));
        public void SetXLeftValue()
        {
            string inputText = GameObject.Find("xLeftValue").GetComponent<TMP_InputField>().text;
            if (UseXLog)
                inputText = Mathf.CeilToInt(float.Parse(inputText)).ToString();
            CheckDecimals(inputText);
            this.xLeftValue = float.Parse(inputText);
            inputValuesChanged = true;
        }

        public void SetXStepSize()
        {
            string inputText = GameObject.Find("xStepSize").GetComponent<TMP_InputField>().text;
            CheckDecimals(inputText);
            this.xStepSize = float.Parse(inputText);
            inputValuesChanged = true;
        }

        public void SetXRightValue()
        {
            string inputText = GameObject.Find("xRightValue").GetComponent<TMP_InputField>().text;
            if (UseXLog)
                inputText = Mathf.CeilToInt(float.Parse(inputText)).ToString();
            CheckDecimals(inputText);
            this.xRightValue = float.Parse(inputText);
            inputValuesChanged = true;
        }

        public void SetYLeftValue()
        {
            string inputText = GameObject.Find("yLeftValue").GetComponent<TMP_InputField>().text;
            if (UseYLog)
                inputText = Mathf.CeilToInt(float.Parse(inputText)).ToString();
            CheckDecimals(inputText);
            this.yLeftValue = float.Parse(inputText);
            inputValuesChanged = true;
        }

        public void SetYStepSize()
        {
            string inputText = GameObject.Find("yStepSize").GetComponent<TMP_InputField>().text;
            CheckDecimals(inputText);
            this.yStepSize = float.Parse(inputText);
            inputValuesChanged = true;
        }

        public void SetYRightValue()
        {
            string inputText = GameObject.Find("yRightValue").GetComponent<TMP_InputField>().text;
            if (UseYLog)
                inputText = Mathf.CeilToInt(float.Parse(inputText)).ToString();
            CheckDecimals(inputText);
            this.yRightValue = float.Parse(inputText);
            inputValuesChanged = true;
        }
        public void SetYLogValue()
        {
            bool isLog = GameObject.Find("yLogToggle").GetComponent<Toggle>().isOn;
            this.UseYLog = isLog;
            ResetYAxis();
            inputValuesChanged = true;
        }
        public void SetXLogValue()
        {
            bool isLog = GameObject.Find("xLogToggle").GetComponent<Toggle>().isOn;
            this.UseXLog = isLog;
            ResetXAxis();
            inputValuesChanged = true;
        }
        public bool useXLog => UseXLog;
        public bool useYLog => UseYLog;
        public void CreateDataPlot()
        {
            if (TextEditing.TextIsNotSet("xLeftValue")) Debug.LogError("xLeftValue not set");
            else if (TextEditing.TextIsNotSet("xStepSize")) Debug.LogError("xStepSize not set");
            else if (TextEditing.TextIsNotSet("xRightValue")) Debug.LogError("xRightValue not set");
            else if (TextEditing.TextIsNotSet("yLeftValue")) Debug.LogError("yLeftValue not set");
            else if (TextEditing.TextIsNotSet("yStepSize")) Debug.LogError("yStepSize not set");
            else if (TextEditing.TextIsNotSet("yRightValue")) Debug.LogError("yRightValue not set");
            else if (xLeftValue == xRightValue) Debug.LogError("xLeftValue can't be the same as xRightValue");
            else if (yLeftValue == yRightValue) Debug.LogError("yLeftValue can't be the same as yRightValue");
            else if (xLeftValue > xRightValue) Debug.LogError("xLeftValue can't greater than xRightValue");
            else if (yLeftValue > yRightValue) Debug.LogError("yLeftValue can't greater than yRightValue");
            else if (xStepSize == 0) Debug.LogError("xStepSize can't be 0");
            else if (yStepSize == 0) Debug.LogError("yStepSize can't be 0");
            else GeneratePlot();
        }

        private void GeneratePlot()
        {
            GameObject.Find("CreatePlotButton").SetActive(false);
            axis.SetActive(true);
            axisScript = GameObject.Find("Axis").GetComponent<AxisScript>();
            //MoveStepSizeInputFields();
            ActivateLogToggle();
            ActivateInfoText();
            initalValuesAllSet = true;
            _isActive = true;
        }

        private void MoveStepSizeInputFields()
        {
            float canvasWidth = GameObject.Find("DataPlotCanvas").GetComponent<RectTransform>().rect.width;
            float canvasHeight = GameObject.Find("DataPlotCanvas").GetComponent<RectTransform>().rect.height;
            Vector2 bottomLeft = new Vector2(-canvasWidth / 2, -canvasHeight / 2);

            Vector3 xStepSizePos = new Vector3(bottomLeft.x + 80, bottomLeft.y + 15);
            TextEditing.ChangeAchoredPosition3D("xStepSize", xStepSizePos);

            Vector3 yStepSizePos = new Vector3(canvasWidth / 2 - 80, bottomLeft.y + 15);
            TextEditing.ChangeAchoredPosition3D("yStepSize", yStepSizePos);
        }


        void Awake()
        {
            transform.gameObject.AddComponent<Image>();
            mask = transform.gameObject.AddComponent<RectMask2D>();
            mask.padding = new(x: xLeftValue, w: yLeftValue, z: xRightValue, y: yRightValue);
        }

        void Start()
        {
            if (useDefaultSettings) SetUpWithDefaultSettings();
        }

        private void FixedUpdate()
        {
            if (inputValuesChanged && initalValuesAllSet)
            {
                axisScript.singleSectorAxis = (useXLog || UseYLog);
                //Debug.Log("xDefaultUseLog || xDefaultUseLog"+ useXLog + " "+ UseYLog + " => "+ axisScript.singleSectorAxis);
                axisScript.RedrawAxis();
                Refresh();
                inputValuesChanged = false;
            }

            if (axis.activeInHierarchy && axisScript.redrawTickLabels)
            {
                GameObject.Find("TickLabels").GetComponent<TickLabelScirpt>().controllerScript = this;
                GameObject.Find("TickLabels").GetComponent<TickLabelScirpt>().RedrawTickLabels();
                axisScript.redrawTickLabels = false;
            }
            if (axisScript != null && updateMask)
                SetMask(axisScript.leftSpace, axisScript.rightSpace, axisScript.topSpace, axisScript.bottomSpace);
        }

        private void CheckDecimals(string value)
        {
            if (
                !TextEditing.TextIsNotSet("xLeftValue") &&
                !TextEditing.TextIsNotSet("xStepSize") &&
                !TextEditing.TextIsNotSet("xRightValue"))
            {
                xMaxDecimalAmount = 0;
                string xLeft = GameObject.Find("xLeftValue").GetComponent<TMP_InputField>().text;
                int xLeftDecimals = TextEditing.GetDecimalsAmount(xLeft);
                string xRight = GameObject.Find("xRightValue").GetComponent<TMP_InputField>().text;
                int xRightDecimals = TextEditing.GetDecimalsAmount(xRight);
                string xStep = GameObject.Find("xStepSize").GetComponent<TMP_InputField>().text;
                int xStepDecimals = TextEditing.GetDecimalsAmount(xStep);
                //Debug.Log(xLeftDecimals);
                //Debug.Log(xRightDecimals);
                //Debug.Log(xStepDecimals);

                if (xLeftDecimals > xMaxDecimalAmount) xMaxDecimalAmount = xLeftDecimals;
                if (xRightDecimals > xMaxDecimalAmount) xMaxDecimalAmount = xRightDecimals;
                if (xStepDecimals > xMaxDecimalAmount) xMaxDecimalAmount = xStepDecimals;
            }
            if (
                !TextEditing.TextIsNotSet("yLeftValue") &&
                !TextEditing.TextIsNotSet("yStepSize") &&
                !TextEditing.TextIsNotSet("yRightValue"))
            {
                yMaxDecimalAmount = 0;
                string yLeft = GameObject.Find("yLeftValue").GetComponent<TMP_InputField>().text;
                int yLeftDecimals = TextEditing.GetDecimalsAmount(yLeft);
                string yRight = GameObject.Find("yRightValue").GetComponent<TMP_InputField>().text;
                int yRightDecimals = TextEditing.GetDecimalsAmount(yRight);
                string yStep = GameObject.Find("yStepSize").GetComponent<TMP_InputField>().text;
                int yStepDecimals = TextEditing.GetDecimalsAmount(yStep);

                if (yLeftDecimals > yMaxDecimalAmount) yMaxDecimalAmount = yLeftDecimals;
                if (yRightDecimals > yMaxDecimalAmount) yMaxDecimalAmount = yRightDecimals;
                if (yStepDecimals > yMaxDecimalAmount) yMaxDecimalAmount = yStepDecimals;
            }
        }


        List<IPlot2D> plotList2D = new();
        List<IPlot2D> IAxis2D.plotList => plotList2D;

        List<IPlot3D> plotList3D = new();
        List<IPlot3D> IAxis3D.plotList => plotList3D;

        IGraph2D graph2D;
        IGraph3D graph3D;
        IGraph2D IAxis2D.graph { get => graph2D; set => graph2D = value; }
        IGraph3D IAxis3D.graph { get => graph3D; set => graph3D = value; }

        public IGraph graph => (IGraph)graph2D ?? graph3D;

        IPlot2D IAxis2D.AddPlot(IFunction2D function) => AddPlot(function);
        IPlot3D IAxis3D.AddPlot(IFunction3D function) => AddPlot(function);
        LinePlot AddPlot(IFunction2D function)
        {
            LinePlot plot = LinePlot.Create(function, this);
            plotList2D.Add(plot);
            return plot;
        }
        HeatMap AddPlot(IFunction3D function)
        {
            HeatMap plot = HeatMap.Create(function, this);
            plotList3D.Add(plot);
            return plot;
        }

        Vector3 IAxis.GetAxisZero() => GetAxisZero();
        Vector3 GetAxisZero()
        {
            return (Vector3)axisScript.GetZeroZeroPosition() + axis.transform.position;
        }

        Vector2 IAxis.GetAxisZero2D() => GetAxisZero2D();
        Vector2 GetAxisZero2D()
        {
            Vector2 axisZero = Vector2.zero;
            if (axisScript == null)
                return axisZero;

            axisZero = axisScript.GetZeroZeroPosition();
            return axisZero;

        }

        Vector2 IAxis.GetScale() => GetScale();
        Vector2 GetScale()
        {
            Vector2 canvasScale = displayedSpace();
            Vector2 axisScale = Vector2.one;
            if (axisScript == null)
                return axisScale;
            axisScale = axisScript.GetAxisSize();
            Vector2 scale = new Vector2(axisScale.x / canvasScale.x, axisScale.y / canvasScale.y);
            return scale;

            Vector2 displayedSpace()
            {
                float[] intervall = ((IAxis2D)this).GetXAxisIntervall();
                float xDistance = Mathf.Abs(intervall[1] - intervall[0]);
                intervall = ((IAxis2D)this).GetYAxisIntervall();
                float yDistance = Mathf.Abs(intervall[1] - intervall[0]);
                //Debug.Log(xDistance + " " + yDistance);
                return new Vector2(xDistance, yDistance);
            }
        }

        Vector2[] IAxis.GetAxisOffset() => GetAxisOffset();
        Vector2[] GetAxisOffset()
        {
            Vector2[] offset = new Vector2[4];
            if (axisScript == null)
                return offset;

            offset = new[]
            {
            new Vector2(axisScript.xCalculatedStartPosition.x, axisScript.yCalculatedStartPosition.y),
            new Vector2(axisScript.xCalculatedEndPosition.x, axisScript.yCalculatedEndPosition.y)
        };
            return offset;
        }


        Vector2[] IAxis.GetAxisWorldCorners() => GetAxisWorldCorners();
        Vector2[] GetAxisWorldCorners()
        {
            Vector2[] worldCorners = new Vector2[4];
            if (axisScript == null)
                return worldCorners;

            Vector2 axisSize = axisScript.GetAxisSize() / 2;

            worldCorners = new Vector2[] {
            new(transform.parent.position.x-axisSize.x,transform.parent.position.y-axisSize.y),//Bottom Left
            new(transform.parent.position.x-axisSize.x,transform.parent.position.y+axisSize.y),//Top Left
            new(transform.parent.position.x+axisSize.x,transform.parent.position.y+axisSize.y),//Top Right
            new(transform.parent.position.x+axisSize.x,transform.parent.position.y-axisSize.y),//Bottom Right
        };
            return worldCorners;
        }

        Vector2[] IAxis.GetAxisCorners() => GetAxisCorners();
        Vector2[] GetAxisCorners()
        {
            Vector2 axisSize = axisScript.canvasSize / 2;
            Vector2[] axisCorners = new Vector2[4];

            axisCorners[0] = new Vector2(-axisSize.x + axisScript.xDrawStartPosition.x, -axisSize.y + axisScript.yDrawStartPosition.y);//Bottom Left
            axisCorners[1] = new Vector2(-axisSize.x + axisScript.xDrawStartPosition.x, -axisSize.y + axisScript.yDrawEndPosition.y);//Top Left
            axisCorners[2] = new Vector2(-axisSize.x + axisScript.xDrawEndPosition.x, -axisSize.y + axisScript.yDrawEndPosition.y);//Top Right
            axisCorners[3] = new Vector2(-axisSize.x + axisScript.xDrawEndPosition.x, -axisSize.y + axisScript.yDrawStartPosition.y);//Bottom Right
            return axisCorners;
        }

        float[] IAxis.GetXAxisIntervall() => GetXAxisIntervall();
        float[] GetXAxisIntervall()
        {
            return new float[] { xLeftValue, xRightValue };
        }

        float[] IAxis.GetYAxisIntervall() => GetYAxisIntervall();
        float[] GetYAxisIntervall()
        {
            return new float[] { yLeftValue, yRightValue };
        }

        Vector2 IAxis.GetStepSize() => new(x: xStepSize, y: yStepSize);

        void SetMask(float left, float right, float top, float down)
        {
            mask.padding = new(x: left, w: top, z: right, y: down);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (axisScript)
                foreach (Vector2 pos in GetAxisCorners())
                {
                    Gizmos.DrawWireSphere(pos, 15f);
                }
            Gizmos.color = Color.green;
        }

        private void ActivateInfoText()
        {
            GameObject.Find("xLeftValue").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("xStepSize").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("xRightValue").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("yLeftValue").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("yStepSize").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("yRightValue").transform.GetChild(1).gameObject.SetActive(true);
        }

        private void ActivateLogToggle()
        {
            GameObject.Find("LogToggleButtons").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("LogToggleButtons").transform.GetChild(1).gameObject.SetActive(true);
        }

        private void SetUpWithDefaultSettings()
        {
            ActivateLogToggle();
            if (xDefaultUseLog)
            {
                GameObject.Find("xLeftValue").GetComponent<TMP_InputField>().text = xDefaultExpStart.ToString();
                GameObject.Find("xStepSize").GetComponent<TMP_InputField>().text = xDefaultStepSize.ToString();
                GameObject.Find("xRightValue").GetComponent<TMP_InputField>().text = xDefaultExpEnd.ToString();
                GameObject.Find("xLogToggle").GetComponent<Toggle>().isOn = xDefaultUseLog;
            }
            else
            {
                GameObject.Find("xLeftValue").GetComponent<TMP_InputField>().text = xDefaultLeftValue.ToString();
                GameObject.Find("xStepSize").GetComponent<TMP_InputField>().text = xDefaultStepSize.ToString();
                GameObject.Find("xRightValue").GetComponent<TMP_InputField>().text = xDefaultRightValue.ToString();
            }

            if (yDefaultUseLog)
            {
                GameObject.Find("yLeftValue").GetComponent<TMP_InputField>().text = yDefaultExpStart.ToString();
                GameObject.Find("yStepSize").GetComponent<TMP_InputField>().text = yDefaultStepSize.ToString();
                GameObject.Find("yRightValue").GetComponent<TMP_InputField>().text = yDefaultExpEnd.ToString();
                GameObject.Find("yLogToggle").GetComponent<Toggle>().isOn = yDefaultUseLog;
            }
            else
            {
                GameObject.Find("yLeftValue").GetComponent<TMP_InputField>().text = yDefaultLeftValue.ToString();
                GameObject.Find("yStepSize").GetComponent<TMP_InputField>().text = yDefaultStepSize.ToString();
                GameObject.Find("yRightValue").GetComponent<TMP_InputField>().text = yDefaultRightValue.ToString();
            }

            SetXLeftValue();
            SetXStepSize();
            SetXRightValue();
            SetYLeftValue();
            SetYStepSize();
            SetYRightValue();
            CreateDataPlot();
        }

        public Vector3 TranslateToWorld(Vector2 point)
        {
            Vector2 scale = GetScale();
            Vector2 transPoint = point;
            if (useXLog)
            {
                transPoint.x = (float.IsFinite(Mathf.Log10(point.x))) ? Mathf.Log10(point.x) : point.x < 0 ? xLeftValue - 1 : Mathf.Sign(point.x) * Mathf.Log10(1 + Mathf.Abs(point.x) / constantLog);
            }
            if (useYLog)
            {
                transPoint.y = (float.IsFinite(Mathf.Log10(point.y))) ? Mathf.Log10(point.y) : point.y < 0 ? yLeftValue - 1 : Mathf.Sign(point.y) * Mathf.Log10(1 + Mathf.Abs(point.y) / constantLog);
            }
            transPoint *= scale;
            Vector2 translated = (transPoint + GetAxisZero2D());
            return translated;
        }

        public Vector2 TranslateToAxis(Vector3 point)
        {
            Vector2 translated = new Vector2(x: (point).x, y: (point).y) - GetAxisZero2D();
            translated /= GetScale();
            if (useXLog)
            {
                translated.x = Mathf.Pow(10, translated.x);
            }
            if (useYLog)
            {
                translated.y = Mathf.Pow(10, translated.y);
            }
            return translated;

        }

        void ResetXAxis()
        {
            if (UseXLog)
            {
                GameObject.Find("xLeftValue").GetComponent<TMP_InputField>().text = xDefaultExpStart.ToString();
                GameObject.Find("xStepSize").GetComponent<TMP_InputField>().text = xDefaultStepSize.ToString();
                GameObject.Find("xRightValue").GetComponent<TMP_InputField>().text = xDefaultExpEnd.ToString();
            }
            else
            {
                GameObject.Find("xLeftValue").GetComponent<TMP_InputField>().text = xDefaultLeftValue.ToString();
                GameObject.Find("xStepSize").GetComponent<TMP_InputField>().text = xDefaultStepSize.ToString();
                GameObject.Find("xRightValue").GetComponent<TMP_InputField>().text = xDefaultRightValue.ToString();
            }
            SetXLeftValue();
            SetXStepSize();
            SetXRightValue();

        }
        void ResetYAxis()
        {
            if (UseYLog)
            {
                GameObject.Find("yLeftValue").GetComponent<TMP_InputField>().text = yDefaultExpStart.ToString();
                GameObject.Find("yStepSize").GetComponent<TMP_InputField>().text = yDefaultStepSize.ToString();
                GameObject.Find("yRightValue").GetComponent<TMP_InputField>().text = yDefaultExpEnd.ToString();
            }
            else
            {
                GameObject.Find("yLeftValue").GetComponent<TMP_InputField>().text = yDefaultLeftValue.ToString();
                GameObject.Find("yStepSize").GetComponent<TMP_InputField>().text = yDefaultStepSize.ToString();
                GameObject.Find("yRightValue").GetComponent<TMP_InputField>().text = yDefaultRightValue.ToString();
            }
            SetYLeftValue();
            SetYStepSize();
            SetYRightValue();

        }


        public void Refresh()
        {
            foreach (IPlot plot in plotList2D)
            {
                plot.Refresh();
            }
            foreach (IPlot plot in plotList3D)
            {
                plot.Refresh();
            }
        }
    }

}