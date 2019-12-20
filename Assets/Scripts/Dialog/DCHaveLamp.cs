using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//One of the most basic dialog conditions possible (while still being useful). with kind of a dumb name.
//This could be made into a more generic 'boolean condition' condition, but I'll leave it as is for now to see how this works in practice
public class DCHaveLamp : IDialogCondition
{
    private bool lamp_state_needed_ = true;
    private bool has_lamp_ = false;

    //TODO: I think the way this has to work is that Evaluate should 'look up' the information needed, rather than having the condition store state itself.
    public bool Evaluate()
    {
        return lamp_state_needed_ == has_lamp_;
    }

    public void SetRequirement(string lamp_status)
    {
        //Could change this to be put in the interface without using templates? or do an abstract class or something?
        //else, as it is now, it's up to each individual condition to convert from the string to actual value
        bool lamp_status_bool = true;
        bool.TryParse(lamp_status, out lamp_status_bool);
        lamp_state_needed_ = lamp_status_bool ;
    }

    public Type GetRequirementType()
    {
        return lamp_state_needed_.GetType();
    }

    public void SetLamp(bool has_lamp)
    {
        has_lamp_ = has_lamp;
    }

}
