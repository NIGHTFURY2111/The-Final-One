using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ST_debug : MonoBehaviour
{
    static ST_debug instance;
    [SerializeField] TMP_Text debugText;

    string _displayString = "";

    public static string displayString
    {
        get => instance._displayString;
        set => instance._displayString = value + "\n";
    }
    public ST_debug()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //public void AddString(string str)
    //{
    //    debugText.text += str + "\n";
    //}

    private void Update()
    {
        debugText.text = displayString;
    }


}
