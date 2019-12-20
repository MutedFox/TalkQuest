using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEdge
{
    // The node reached upon termination of this edge.
    public DialogNode out_node_ { get; set; }

    // If any of these conditions evaluate to false, this dialog edge is not available.
    private List<string> required_flags_;
    private List<string> required_flag_values_;

    // These are the flags set when this edge is traversed.
    private List<string> flags_to_set_;
    private List<string> flag_values_to_set_;

    // The content of this edge containing a line of text, a portrait and the name of the speaker
    public IDialogLine dialog_line_ { get; set; }

    // Scripts to run for special behaviour
    public List<string> scripts_ { get; set; }

    // When a node has multiple edges it can transition across, it should ignore all edges that are not the highest priority from that node
    public int priority_ { get; set; }

    // reference to 'condition handler' or 'player status' etc. to check for dialog conditions
    private GameObject condition_evaluator_;

   public DialogEdge(DialogNode out_node)
    {
        out_node_ = out_node;
        required_flags_ = new List<string>();
        required_flag_values_ = new List<string>();
        flags_to_set_ = new List<string>();
        flag_values_to_set_ = new List<string>();
        scripts_ = new List<string>();
        condition_evaluator_ = GameObject.Find("Player Status");
    }

    // Check all conditions to see whether this edge can be traversed.
    public bool IsAvailable()
    {
        for (int i = 0; i < required_flags_.Count; i++)
        {
            if (condition_evaluator_.GetComponent<IConditionHandler>().EvaluateFlag(required_flags_[i], required_flag_values_[i]) == false) 
                return false;
        }
        
        return true;
    }

    // To be called when this edge is traversed; updates all flags in flags_to_set_ with their corresponding values.
    public void UpdateFlags()
    {
        for (int i = 0; i < flags_to_set_.Count; i++) {
            condition_evaluator_.GetComponent<IConditionHandler>().UpdateFlag(flags_to_set_[i], flag_values_to_set_[i]);
        }
    }

    public void AddRequiredFlag(string flag, string flag_value)
    {
        required_flags_.Add(flag);
        required_flag_values_.Add(flag_value);
    }

    public void AddFlagToSet(string flag, string flag_value)
    {
        flags_to_set_.Add(flag);
        flag_values_to_set_.Add(flag_value);
    }

    public void AddScript(string script_name)
    {
        scripts_.Add(script_name);
    }

    public void RunScripts()
    {

    }
}
