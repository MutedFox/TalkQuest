using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputPriority
{
    DEFAULT,
    DIALOG_BOX,
    DIALOG_CHOICE,
    MENU
}

// This script handles inputs and looks at 'priority' e.g. so the player can't move while in a dialog
// basically that sort of logic is put in here (this way it's easy for there to be more 'layers' without 
// input affecting multiple simultaneously
// normal movement/gameplay -> dialog box -> dialog choice -> menu etc. 

// This class acts as the 'main input hub' that any object that needs inputs will talk to
public class InputManager : MonoBehaviour
{
    // The highest priority of input going on right now
    private InputPriority highest_priority_ = InputPriority.DEFAULT;

    // track how many of each priority there are (to more easily switch between what is accepting input)
    private Dictionary<InputPriority, int> priority_counts_;

    void Awake()
    {
        priority_counts_ = new Dictionary<InputPriority, int>();
    }

    public bool IsSufficientPriority(InputPriority priority)
    {
        if (highest_priority_ <= priority)
        {
            return true;
        }

        return false;
    }

    // Track a new object's priority (this should be called by that object on instantiation)
    public void AddPriority(InputPriority priority)
    {
        // If this is the first time on object with this priority has been seen, add to the dict
        if (!priority_counts_.ContainsKey(priority))
            priority_counts_.Add(priority, 0);

        priority_counts_[priority]++;
        if (priority > highest_priority_)
            highest_priority_ = priority;
    }

    // When an object that accepts input stops existing, it should call this function to update the current highest priority
    public void RemovePriority(InputPriority priority)
    {
        priority_counts_[priority]--;
        while(priority_counts_[highest_priority_] == 0)
        {
            // If no objects exist that accept inputs exist, stop, though that scenario shouldn't normally happen
            if (highest_priority_ == InputPriority.DEFAULT)
                break;
            highest_priority_--;
        }
    }
}
