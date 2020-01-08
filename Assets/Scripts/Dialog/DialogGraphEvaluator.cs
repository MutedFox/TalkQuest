using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogGraphEvaluator : MonoBehaviour
{
    private DialogGraph dialog_graph_;

    private DialogNode current_node_;
    private DialogEdge current_edge_;   // unneeded? just return the edge that's picked when advancing to a node

    // The dialog box is where the main results of the evaluator are displayed. 
    public GameObject dialog_box_prefab_;
    private GameObject dialog_box_instance_ = null;

    // A choice box is created when a choice in the dialog is to be made
    public GameObject dialog_choice_box_prefab_;
    private GameObject dialog_choice_box_instance_ = null;

    // When a dialog choice is to be made, these track the current selection.
    private int total_choices_ = 0;
    private int current_choice_ = 0;

    private InputRetriever input_;

    // Start is called before the first frame update
    void Awake()
    {
        dialog_box_instance_ = Instantiate(dialog_box_prefab_);
        input_ = GetComponent<InputRetriever>();
    }

    // Update is called once per frame
    void Update()
    {
        if(dialog_choice_box_instance_ != null)
        {
            if (input_.GetAxisSlow("Vertical") < 0 && current_choice_ < total_choices_ - 1)
                dialog_choice_box_instance_.GetComponent<IDialogChoiceBox>().ChangeSelection(++current_choice_);
            else if (input_.GetAxisSlow("Vertical") > 0 && current_choice_ > 0)
                dialog_choice_box_instance_.GetComponent<IDialogChoiceBox>().ChangeSelection(--current_choice_);
            else if (input_.GetButtonDown("Fire1"))
                ChooseNode(current_choice_);

        }
        else if(input_.GetButtonDown("Fire1"))
        {
            // If the dialog is still being typed out, skip to the end immediately. TODO: another button that advances node and ignores typing?
            if (dialog_box_instance_.GetComponent<DialogBox>().is_typing_)
                dialog_box_instance_.GetComponent<DialogBox>().FinishDialogTyping();
            else
                AdvanceNode();
        }
    }

    public void Initialise(DialogGraph dialog_graph)
    {
        dialog_graph_ = dialog_graph;
        current_node_ = dialog_graph_.GetRootNode();

        // the first node should never have a player choice, and should progress automatically so there isn't a blank text box
        AdvanceNode();

    }

    // Advance along graph function  (update current node/edge)
    private void ChooseNode(int choice)
    {
        // If the conditions for this choice have not been met, nothing will happen
        DialogEdge edge = current_node_.GetOutgoingEdges()[choice];
        if (edge.IsAvailable())
        {
            current_node_ = edge.out_node_;      //TODO: handle cases where terminal node
            current_edge_ = edge;

            // Get rid of the dialog choice box and reset the current choice selection
            Destroy(dialog_choice_box_instance_);
            dialog_choice_box_instance_ = null;
            total_choices_ = 0;
            current_choice_ = 0;

            // Update any flags in the traversed edge
            edge.UpdateFlags();

            // Activate any scripts for the traversed edge (TODO: script variables? may be unnecessary; also: maybe put this into its own function if variables)
            foreach (string script_name in edge.scripts_)
                SendMessage(script_name);

            // Assuming the choice didn't lead directly to another choice, advance node again so the correct followup dialog displays.
            CheckForChoices();
            if (dialog_choice_box_instance_ == null)
                AdvanceNode();

        }
    }

    // When the current node isn't a choice, this handles the result of the player advancing the dialog
    private void AdvanceNode()
    {
        if (current_node_.IsTerminal())
        {
            ExitDialog();
            return;
        }

        DialogEdge edge_to_traverse = null;
        if(current_node_.GetOutgoingEdges().Count == 1)
        {
            edge_to_traverse = current_node_.GetOutgoingEdges()[0];
        }
        else
        {
            foreach(DialogEdge edge in current_node_.GetOutgoingEdges())
            {
                // If this edge is available to traverse (its conditions are met), and it's the highest priority edge seen so far,
                // set it as the one to traverse.
                if ((edge_to_traverse == null || edge.priority_ > edge_to_traverse.priority_) && edge.IsAvailable())
                    edge_to_traverse = edge;
            }
        }

        // Update what the dialog box displays
        dialog_box_instance_.GetComponent<DialogBox>().UpdateDialog(edge_to_traverse.dialog_line_.GetLineBody(), edge_to_traverse.dialog_line_.GetData()[0]);
        current_node_ = edge_to_traverse.out_node_;

        // Update any flags in the traversed edge
        edge_to_traverse.UpdateFlags();

        // Activate any scripts for the traversed edge (TODO: script variables? may be unnecessary)
        foreach (string script_name in edge_to_traverse.scripts_)
            SendMessage(script_name);

        // Check if the node we arrive at is a choice
        CheckForChoices();
    }

    // When a new dialog node is reached, this method determines whether it's choice node (for the player to choose) and if so creates creates a DialogChoiceBox.
    private void CheckForChoices()
    {
        if (current_node_.is_choice_)
        {
            int highest_priority = -1;
            List<string> choices = new List<string>();
            foreach (DialogEdge edge in current_node_.GetOutgoingEdges())
            {
                if (edge.IsAvailable())
                {
                    // If this is the highest priority seen so far, reset the choice count and list of choices
                    if (edge.priority_ > highest_priority)
                    {
                        highest_priority = edge.priority_;
                        total_choices_ = 1;
                        choices = new List<string>() { edge.dialog_line_.GetLineBody() };
                    }
                    // If this is the same priority as the highest seen, then it's another choice
                    else if (edge.priority_ == highest_priority)
                    {
                        total_choices_++;
                        choices.Add(edge.dialog_line_.GetLineBody());
                    }
                }

            }
            // TODO: currently non available choices don't appear at all but it may be good if they appear greyed out or something
            dialog_choice_box_instance_ = Instantiate(dialog_choice_box_prefab_);
            dialog_choice_box_instance_.GetComponent<IDialogChoiceBox>().Initialise(choices);
        }
    }

    // Clean up and exit the conversation.
    private void ExitDialog()
    {
        Destroy(dialog_box_instance_);
        if (dialog_choice_box_instance_ != null)
            Destroy(dialog_choice_box_instance_);

        input_.Destroy();
    }
}
