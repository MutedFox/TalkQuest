using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzConverter : MonoBehaviour
{
    public string input;

    // Start is called before the first frame update
    void Start()
    {
        string output = "";
        foreach(char ch in input)
        {
            if (ch.Equals('.'))
                output += "bz";
            else if (ch.Equals('-'))
                output += "buzz";
            else
                output += ch;
        }
        Debug.Log(output);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
