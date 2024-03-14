using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HelperScirpts;

public class TestScript : MonoBehaviour
{
    public string value = "";
    public int decimals;
    public bool convert;
    // Update is called once per frame
    void Update()
    {
        if(convert)
        {
            convert = false;
            string outout = TextEditing.CorrectDeciamlsAmount(value, decimals);
            Debug.Log("Input: " + value);
            Debug.Log("Output: " + outout);
        }
    }
}
