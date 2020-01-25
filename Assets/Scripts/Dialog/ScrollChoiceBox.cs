using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Implementation of IDialogChoiceBox where there is a single panel containing all the choices that can be scrolled through (with up/down buttons)
public class ScrollChoiceBox : MonoBehaviour, IDialogChoiceBox
{
    public GameObject text_box_prefab_;
    private List<GameObject> text_box_instances_;

    List<string> choices_;
    int current_choice_ = 0;

    public string pointer_move_sound_;

    // The object with the Scroll Rect component 
    private GameObject scroll_view_;

    // Object containing the choices objects; has a VerticalLayoutGroup component
    private GameObject choice_group_;   

    // The scrollbar that the player doesn't move themselves but if there are enough choices, will move up and down with the choice selection
    private Scrollbar scroll_bar_;

    // The object that points at the current selection
    private GameObject pointer_;

    // Used to move the pointer appropriately; is the the amount of height needed for one choice shown in the box
    private float text_height_per_choice_;

    private SoundManager sound_manager_;

    void Awake()
    {
        scroll_view_ = transform.Find("Scroll Choice Box/Panel/Scroll View").gameObject;
        choice_group_ = transform.Find("Scroll Choice Box/Panel/Scroll View/Viewport/Choice Box Content").gameObject;
        scroll_bar_ = scroll_view_.GetComponent<ScrollRect>().verticalScrollbar;
        pointer_ = transform.Find("Scroll Choice Box/Panel/Finger Pointer").gameObject;

        sound_manager_ = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
    }

    // Populate the choice text box objects (and number them)
    public void Initialise(List<string> choices)
    {
        text_box_instances_ = new List<GameObject>();
        choices_ = choices;
        int count = 1;
        foreach (string choice in choices_)
        {
            GameObject text_box = Instantiate(text_box_prefab_);
            text_box.transform.SetParent(choice_group_.transform, false);
            text_box.GetComponentInChildren<Text>().text = count.ToString() + ". " + choice;
            text_box_instances_.Add(text_box);

            count++;
        }

        scroll_bar_.value = 1f;
        AdjustPointer();
    }

    // Adjust the scroll bar to display the currently selected choice and also adjust the pointer to point to it.
    public void ChangeSelection(int choice)
    {
        current_choice_ = choice;

        // height of all choices, height of choices above current_choice, and height of the viewport respectively
        float total_height = choice_group_.GetComponent<RectTransform>().rect.height;
        float height_of_above_choices = HeightOfAboveChoices();
        float displayed_height = scroll_view_.GetComponent<RectTransform>().rect.height;
        
        // top of the VerticalLayoutGroup object (i.e. top of the top choice), top of current_choice, and bottom of current_choice respectively
        float group_top = choice_group_.GetComponent<RectTransform>().anchoredPosition.y;
        float choice_top = group_top + height_of_above_choices;
        float choice_bottom = choice_top + text_box_instances_[current_choice_].GetComponent<RectTransform>().rect.height;

        // If current_choice_ is the top or bottom in the list, the viewport should always be scrolled to the maximum/minimum value.
        // Otherwise, if a choice is already completely contained in the displayed area, don't move.
        // If a choice is not completely contained, scroll the viewport the minimum amount to display it completely. 
        // (by comparing the tops/bottoms of the current choice and viewport)
        if (current_choice_ == 0)
            scroll_bar_.value = 1;
        else if (current_choice_ == choices_.Count - 1)
            scroll_bar_.value = 0;
        else if (choice_top > scroll_view_.GetComponent<RectTransform>().anchoredPosition.y + scroll_view_.GetComponent<RectTransform>().rect.height / 2)
            scroll_bar_.value = (total_height - height_of_above_choices - displayed_height) / (total_height - displayed_height);
        else if (choice_bottom < scroll_view_.GetComponent<RectTransform>().anchoredPosition.y - scroll_view_.GetComponent<RectTransform>().rect.height / 2)
            scroll_bar_.value = (total_height - height_of_above_choices - text_box_instances_[current_choice_].GetComponent<RectTransform>().rect.height)
                / (total_height - displayed_height);

        AdjustPointer();
        sound_manager_.PlaySound(pointer_move_sound_);
    }

    // Calculate the height of the choices above the currently selected one
    private float HeightOfAboveChoices()
    {
        float output = 0f;

        for(int i = 0; i < current_choice_; i++)
        {
            output += text_box_instances_[i].GetComponent<RectTransform>().rect.height;
        }

        return output;
    }


    public void SubmitChoice()
    {

    }

    // Moves the pointer to point at the current selection, based on text_height_per_choice_.
    private void AdjustPointer()
    {
        pointer_.transform.position = new Vector3(pointer_.transform.position.x, text_box_instances_[current_choice_].transform.position.y - text_box_instances_[current_choice_].GetComponent<RectTransform>().rect.height / 2);
    }
}
