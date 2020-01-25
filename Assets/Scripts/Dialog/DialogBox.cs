using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// To be attached to a canvas; handles the display of dialog provided by a DialogGraphEvaluator
public class DialogBox : MonoBehaviour
{
    public GameObject dialog_evaulator_ { get; set; }

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
        if(is_typing_)
        {
            UpdateDialogLine();
            TryTalkBlip();
        }
    }

    // Update the current_line_ and restart the typing out process
    public void UpdateDialog(string line, string speaker = null)
    {
        current_line_ = line;
        current_line_shown_ = "<color=#00000000>"+ line + "</color>";
        is_typing_ = true;
        time_since_char_added_ = 0f;
        characters_shown_ = 0;
        HideAdvanceArrow();

        main_text_.text = current_line_shown_;

        // Hide the speaker box if there's no speaker listed for this line, or reveal it if there is
        if (speaker != null && speaker != "")
        {
            speaker_text_.GetComponentInParent<Image>().color = new Color(255, 255, 255, 255);
            speaker_text_.text = speaker;
        } else
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
            FinishDialogTyping();
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
