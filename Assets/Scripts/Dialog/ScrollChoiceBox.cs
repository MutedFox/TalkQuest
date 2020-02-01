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

    private SoundManager sound_manager_;

    void Awake()
    {
        scroll_view_ = transform.Find("Scroll Choice Box/Panel/Scroll View").gameObject;
        choice_group_ = transform.Find("Scroll Choice Box/Panel/Scroll View/Viewport/Choice Box Content").gameObject;
        scroll_bar_ = scroll_view_.GetComponent<ScrollRect>().verticalScrollbar;
        pointer_ = transform.Find("Scroll Choice Box/Panel/Scroll View/Finger Pointer").gameObject;

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

        // Force a canvas update to guarantee the ContentSizeFitters on the text_box objects have updated
        Canvas.ForceUpdateCanvases();

        scroll_bar_.value = 1f;
        AdjustPointer();
    }

    // Adjust the scroll bar to display the currently selected choice and also adjust the pointer to point to it.
    public void ChangeSelection(int choice)
    {
        current_choice_ = choice;

        RectTransform choice_rect = text_box_instances_[current_choice_].GetComponent<RectTransform>();

        // height of all choices, height of choices above current_choice, and height of the viewport respectively
        float total_height = choice_group_.GetComponent<RectTransform>().rect.height;
        float height_of_above_choices = - choice_rect.anchoredPosition.y;
        float displayed_height = scroll_view_.GetComponent<RectTransform>().rect.height;
        
        // top of the VerticalLayoutGroup object (i.e. top of the top choice), top of current_choice, and bottom of current_choice respectively
        float group_top = choice_group_.GetComponent<RectTransform>().anchoredPosition.y;
        float choice_top = group_top - height_of_above_choices;
        float choice_bottom = choice_top - choice_rect.rect.height;

        // The top and bottom of the visible text area. view_top is taken as the 'origin' so it's 0.
        float view_top = 0;
        float view_bottom = - scroll_view_.GetComponent<RectTransform>().rect.height;

        // If current_choice_ is the top or bottom in the list, the viewport should always be scrolled to the maximum/minimum value.
        // Otherwise, if a choice is already completely contained in the displayed area, don't move.
        // If a choice is not completely contained, scroll the viewport the minimum amount to display it completely. 
        // (by comparing the tops/bottoms of the current choice and viewport)
        if (current_choice_ == 0)
            scroll_bar_.value = 1f;
        else if (current_choice_ == choices_.Count - 1)
            scroll_bar_.value = 0f;
        else if (choice_top > view_top)
            scroll_bar_.value = (total_height - height_of_above_choices - displayed_height) / (total_height - displayed_height);
        else if (choice_bottom < view_bottom)
            scroll_bar_.value = (total_height - height_of_above_choices - choice_rect.rect.height) / (total_height - displayed_height);

        AdjustPointer();
        sound_manager_.PlaySound(pointer_move_sound_);
    }

    // Moves the pointer to point at the current selection
    private void AdjustPointer()
    {
        RectTransform choice_rect = text_box_instances_[current_choice_].GetComponent<RectTransform>();
        RectTransform group_rect = choice_group_.GetComponent<RectTransform>();

        // The first 2 terms move the pointer to the pivot of the current choice.
        // The second 2 terms adjust it so the index finger is roughly pointing at the center of that choice.
        float pointer_y = choice_rect.anchoredPosition.y + group_rect.anchoredPosition.y - choice_rect.rect.height / 2 - pointer_.GetComponent<RectTransform>().rect.height * 2 / 3;

        // When a choice is too large to fit into the text box, this ensures the pointer stays within the viewport height.
        // In future versions this situation would better handled by some kind of resizing on the choice but a choice does have to be very long to get to this point.
        pointer_y = Mathf.Clamp(pointer_y, -scroll_view_.GetComponent<RectTransform>().rect.height - pointer_.GetComponent<RectTransform>().rect.height * 2 / 3, -pointer_.GetComponent<RectTransform>().rect.height);

        pointer_.GetComponent<RectTransform>().anchoredPosition = new Vector3(pointer_.GetComponent<RectTransform>().anchoredPosition.x, pointer_y);
    }
}
