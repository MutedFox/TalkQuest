using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: this could probably just be folded into talkableNPC or something its a bit redundant/excessive atm
public class DialogGraph : MonoBehaviour
{
    //The start node for this conversation
    private DialogNode root_node_;

    //The XML file with this NPC's dialog data
    public string dialog_xml_filename_;

    void Awake()
    {
        root_node_ = GameObject.Find("DialogLoader").GetComponent<DialogFileReader>().GenerateGraph(dialog_xml_filename_);
    }

    public DialogNode GetRootNode()
    {
        return root_node_;
    }
}
