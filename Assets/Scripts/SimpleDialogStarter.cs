using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A dialog can be started from the object this script is attached to, possibly via a context interaction
public class SimpleDialogStarter : MonoBehaviour, IStartsDialog, IContextInteractable
{
    // Object spawned to handle the dialog
    public GameObject dialog_evaluator_prefab_;
    private GameObject dialog_evaluator_instance_ = null;

    // Contains all the information needed about this NPC's dialog
    DialogGraph dialog_graph_;

    // Start is called before the first frame update
    void Start()
    {
        dialog_graph_ = GetComponent<DialogGraph>();
    }

    // Create a dialog evaluator and give it this object's dialog graph to process
    public void StartDialog()
    {
        dialog_evaluator_instance_ = Instantiate(dialog_evaluator_prefab_);
        dialog_evaluator_instance_.GetComponent<DialogGraphEvaluator>().Initialise(dialog_graph_);

        // THIS IS UGLY but also all that's really needed for this tiny project. Would be better to make a Pausable interface etc.
        GameObject.Find("Bee").GetComponent<SineMove>().Pause();
    }

    public void ContextInteraction()
    {
        StartDialog();
    }
}
