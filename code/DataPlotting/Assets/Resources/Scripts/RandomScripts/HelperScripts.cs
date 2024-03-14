using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HelperScirpts
{
    public class RectEditing : MonoBehaviour
    {
        public static void CopyRectTransform(RectTransform rectToCopyFrom, RectTransform rectToPasteTo)
        {
            rectToPasteTo.anchorMin = rectToCopyFrom.anchorMin;
            rectToPasteTo.anchorMax = rectToCopyFrom.anchorMax;
            rectToPasteTo.pivot = rectToCopyFrom.pivot;
            rectToPasteTo.anchoredPosition3D = rectToCopyFrom.anchoredPosition3D;
            rectToPasteTo.sizeDelta = rectToCopyFrom.sizeDelta;
            rectToPasteTo.localScale = rectToCopyFrom.localScale;
        }
    }

    public class TextEditing : MonoBehaviour
    {
        public static bool TextIsNotSet(string gameObjectName)
        {
            return GameObject.Find(gameObjectName).GetComponent<TMP_InputField>().text.Equals("");
        }

        public static void ChangeFontSize(string gameObjectName, int fontSize)
        {
            try
            {
                GameObject.Find(gameObjectName).GetComponent<Transform>().GetChild(0).GetComponent<Transform>().GetChild(2).GetComponent<TMP_Text>().fontSize = fontSize;
            }
            catch(NullReferenceException)
            {
                Debug.Log(GameObject.Find(gameObjectName).GetComponent<Transform>().GetChild(0).GetComponent<Transform>().GetChild(0));
            }
        }

        public static void ChangeDeltaSize(string gameObjectName, Vector2 sizeDelta)
        {
            try
            {
                GameObject.Find(gameObjectName).GetComponent<RectTransform>().sizeDelta = sizeDelta;
            }
            catch (NullReferenceException)
            {
                Debug.Log("GameObject has no RectTransform component attached to it");
            }
        }

        public static void ChangeDeltaSizeToggle(string gameObjectName, Vector2 sizeDelta)
        {
            try
            {
                GameObject.Find(gameObjectName).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = sizeDelta;
            }
            catch (NullReferenceException)
            {
                Debug.Log("GameObject has no RectTransform component attached to it");
            }
        }

        public static void ChangeAchoredPosition3D(string gameObjectName, Vector3 anchoredPosition3D)
        {
            try
            {
                GameObject.Find(gameObjectName).GetComponent<RectTransform>().anchoredPosition3D = anchoredPosition3D;
            }
            catch (NullReferenceException)
            {
                Debug.Log("GameObject has no RectTransform component attached to it");
            }
        }

        public static int GetDecimalsAmount(string value)
        {
            char seperator = DetermineSeparator(value);
            if (seperator == '\0') return 0;

            int seperatorIndex = value.IndexOf(seperator);
            return value.Length - seperatorIndex - 1;
        }

        public static string CorrectDeciamlsAmount(string value, int decimals)
        {
            string output = value;
            char seperator = DetermineSeparator(value);
            
            if (seperator == '\0')
            {
                if(decimals != 0) output += ",";
                for(int i = 0; i < decimals; i++)
                {
                    output += '0';
                }
            }
            else
            {
                if (decimals == 0) output = output.Substring(0, output.IndexOf(seperator) + decimals);
                else output = output.Substring(0, output.IndexOf(seperator) + decimals + 1);
            }
            return output;
        }

        private static char DetermineSeparator(string floatString)
        {
            if (floatString.Contains("."))
            {
                return '.';
            }
            else if (floatString.Contains(","))
            {
                return ',';
            }
            else
            {
                return '\0';
            }
        }
    }
}
