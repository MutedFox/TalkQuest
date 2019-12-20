using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* overall dialog plan
 * 
 * DialogBox handles the primary dialog box and player giving advance inputs
 * make script for DialogOptions; an object with that script is instantiated when an option is present in the dialog
 * DialogBox 'waits' for the options to be resolved
 * 
 * objects with ITalkable when interacted with, spawn a DialogBox object and disable player movement while they're still talking
 * 
 * dialog line and dialog choice
 *  also conditions from outside dialog
 *
 *  a conversation is a directed graph of nodes
 *  each edge has one of line/choice, and maybe some conditions where the path is available
 *  each node is 'blank' (doesn't contain the dialog stuff itself)
 * 
 * need to be able to 'create' a graph easily, and evaluate it easily
 * 
 * creation:
 * have an xml document with the data (I guess?)
 * have a script that on startup, looks at the xml file and constructs the graph and attaches it to the NPC 
 * (as a data structure within itself but NOT as a unity child object or anything because that would be messy)
 * 
 * "evaluating" a graph/conversation:
 * suppose the graph is purely a data structure (i.e. don't make it into some mess of an object with each node a separate instance of the script etc.)
 * 
 * each NPC has their own unique starting node (which may immediately branch if e.g. their conversation changes as the game goes on, etc.)
 * 
 * a talkable NPC has a DialogGraph (with their unique starting node) attached to them
 * When player goes up and talks to them:
 * Spawn a DialogBox and give it the DialogGraph
 * on 'advance dialog', a few possibilties:
 * if a given edge has multiple lines, keep advancing through them 'as normal'
 * on reaching a node, generally there will be options
 * if it's a player choice, need a DialogChoice script or something that appears (exact implementation can vary so long as player can choose, but ideally they still see the previous thing said)
 * in any case then need to go to the appropriate edge and get the lines from that, etc.
 * if any scripts are found along the way, execute them on reaching the edge.
 * 
 * for a node with multiple out edges:
 * if it's the player's turn to speak, should represent a choice (note some choices may be unavailable if e.g. condition not met; must decide whether to show those choices or not (default not))
 * if it's the NPC who has multiple options, it should basically be down to several 'conditions' in which case they need to be prioritised or something to that effect
 */

public class DialogBox : MonoBehaviour
{
    //The text components for the main body of the dialog and the name of the speaker respectively.
    private Text main_text_;
    private Text speaker_text_;

    void Awake()
    {
        main_text_ = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        speaker_text_ = transform.GetChild(1).GetChild(0).GetComponent<Text>();
    }

    //TODO: text 'typing out'? much later. portraits. add indicator that player can advance. etc.
    public void UpdateDialog(string line, string speaker = null)
    {
        main_text_.text = line;
        if (speaker != null)
            speaker_text_.text = speaker;
    }
}
