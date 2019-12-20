using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//A condition for whether a particular edge in a dialog graph can be taken
public interface IDialogCondition
{
    //If false, the edge this condition is attached to may not be taken.
    bool Evaluate();

    //Set some threshold that is required for the condition to evaluate as true.
    void SetRequirement(string value);

    //Return the type of the template
    Type GetRequirementType();
}
