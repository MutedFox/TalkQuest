using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is put on any object that accepts inputs and checks with an InputManager to see if input for that object is currently allowed.
public class InputRetriever : MonoBehaviour
{
    public InputPriority input_priority_ = InputPriority.DEFAULT;

    private GameObject input_handler_;

    // Start is called before the first frame update
    void Start()
    {
        input_handler_ = GameObject.Find("Input Manager");
        input_handler_.GetComponent<InputManager>().AddPriority(input_priority_);
    }

    public float GetAxis(string axis_name)
    {
        if (IsSufficientPriority())
            return Input.GetAxis(axis_name);
        else
            return 0;
    }

    public float GetAxisSlow(string axis_name)
    {
        if (GetButtonDown(axis_name))
            return GetAxis(axis_name);
        else
            return 0;
    }

    public bool GetButtonDown(string button_name)
    {
        if (IsSufficientPriority())
            return Input.GetButtonDown(button_name);
        else
            return false;
    }

    public bool GetButtonUp(string button_name)
    {
        if (IsSufficientPriority())
            return Input.GetButtonUp(button_name);
        else
            return false;
    }

    private bool IsSufficientPriority()
    {
        if (input_handler_.GetComponent<InputManager>().IsSufficientPriority(input_priority_))
            return true;
        else
            return false;
    }
    
    public void Destroy()
    {
        input_handler_.GetComponent<InputManager>().RemovePriority(input_priority_);
        Destroy(gameObject);
    }
}
