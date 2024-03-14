using System;
using UnityEngine;

namespace GraphPlotter.Utilities
{
    public class Utility
    {
        [Obsolete("Use RectEditings version")]
        public static void CopyRectTransform(RectTransform source, RectTransform target)
        {
            target.anchorMin = source.anchorMin;
            target.anchorMax = source.anchorMax;
            target.pivot = source.pivot;
            target.anchoredPosition3D = source.anchoredPosition3D;
            target.sizeDelta = source.sizeDelta;
        }
    }
}