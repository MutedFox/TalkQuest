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
    // The text components for the main body of the dialog and the name of the speaker respectively.
    private Text main_text_;
    private Text speaker_text_;

    // The arrow that appears to indicate the dialog can be advanced (starts transparent).
    private GameObject text_advance_arrow_;

    // Set to true to make the text type out character by character
    public bool is_typing_ { get; private set; } = false;

    // When is_typing_ is true, how fast text is written out in chars per second
    // (TODO: text speed options?)
    private float typing_chars_per_sec_ = 30f;

    // When is_typing_ is true, Time.deltaTime is repeatedly added to this until enough time has passed for main_text_.text to update.
    private float time_since_char_added_ = 0f;

    // How many times per second to play a TalkBlip sound effect while is_typing_ is true.
    private float talk_blip_per_sec_ = 10f;
    private float time_since_talk_blip_ = 0f;

    // The full line of text to display, the part of that line currently being shown, and the number of characters that is respectively
    private string current_line_ = "";
    private string current_line_shown_ = "";
    private int characters_shown_ = 0;

    private GameObject sound_manager_;

    void Awake()
    {
        main_text_ = transform.Find("Dialog Box/Dialog Text").GetComponent<Text>();
        speaker_text_ = transform.Find("Speaker Box/Speaker Text").GetComponent<Text>();
        text_advance_arrow_ = transform.Find("Dialog Box/Text Advance Arrow").gameObject;

        sound_manager_ = GameObject.Find("Game Manager/Sound Manager");
    }

    private void Update()
    {
        // if typing and still stuff left to type
        if(is_typing_)
        {
            UpdateDialogLine();
            TryTalkBlip();
        }
    }

    //TODO: text typing out not 'teleporting' with partially complete words
    public void UpdateDialog(string line, string speaker = null)
    {
        current_line_ = line;
        current_line_shown_ = "<color=#00000000>"+ line + "</color>";
        is_typing_ = true;
        time_since_char_added_ = 0f;
        characters_shown_ = 0;
        HideAdvanceArrow();

        main_text_.text = current_line_shown_;
        if (speaker != null && speaker != "")
        {
            speaker_text_.GetComponentInParent<Image>().color = new Color(255, 255, 255, 255);
            speaker_text_.text = speaker;
        } else // If there's no speaker, hide the speaker box and text
        {
            speaker_text_.GetComponentInParent<Image>().color = new Color(255, 255, 255, 0);
            speaker_text_.text = "";
        }
    }

    // Updates the text currently displayed so the current dialog line appears to be typed out char by char.
    public void UpdateDialogLine()
    {
        int chars_to_add = (int)((Time.deltaTime + time_since_char_added_) * typing_chars_per_sec_);

        if(chars_to_add > 0)
        {
            // effectively shift the start of the color = clear tag along to create the illusion of characters appearing one by one
            current_line_shown_ = current_line_.Substring(0, characters_shown_) + "<color=#00000000>" + 
                current_line_.Substring(characters_shown_, current_line_.Length - characters_shown_) + "</color>"; 
            main_text_.text = current_line_shown_;

            time_since_char_added_ = 0f;
            characters_shown_ += chars_to_add;
        } else
        {
            time_since_char_added_ += Time.deltaTime;
        }

        if (characters_shown_ == current_line_.Length + 1)
        {
            is_typing_ = false;
            ShowAdvanceArrow();
        }
    }

    // Skips the typing out effect of the current dialog line and show the rest of it immediately.
    public void FinishDialogTyping()
    {
        current_line_shown_ = current_line_;
        main_text_.text = current_line_;
        ShowAdvanceArrow();

        is_typing_ = false;
    }

    private void ShowAdvanceArrow()
    {
        text_advance_arrow_.GetComponent<Image>().color = Color.white;
    }

    private void HideAdvanceArrow()
    {
        text_advance_arrow_.GetComponent<Image>().color = Color.clear;
    }

    // If enough time has passed, calls PlayTalkBlip()
    private void TryTalkBlip()
    {
        time_since_talk_blip_ += Time.deltaTime;
        if (time_since_talk_blip_ >= (1f / talk_blip_per_sec_))
        {
            PlayTalkBlip();
            time_since_talk_blip_ = 0f;
        }
    }

    // Plays a random TalkBlip sound effect.
    private void PlayTalkBlip()
    {
        int randno = Random.Range(1, 4);
        sound_manager_.GetComponent<SoundManager>().PlaySound("TalkBlip" + randno.ToString());
    }
}
