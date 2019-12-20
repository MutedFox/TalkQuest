using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiChoiceBox : MonoBehaviour, IDialogChoiceBox
{
    public GameObject panel_prefab_;
    private List<GameObject> panel_instances_;

    private Scrollbar scrollbar_;

    private List<string> choices_;
    private int current_choice_ = 0;

    private SoundManager sound_manager_;

    void Awake()
    {
        sound_manager_ = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
        scrollbar_ = transform.GetChild(0).GetChild(1).GetComponent<Scrollbar>();
    }

    public void Initialise(List<string> choices)
    {
        panel_instances_ = new List<GameObject>();
        choices_ = choices;
        foreach(string choice in choices_)
        {
            GameObject panel = Instantiate(panel_prefab_);
            panel.transform.SetParent(transform.GetChild(0).GetChild(0).GetChild(0));
            panel.GetComponentInChildren<Text>().text = choice;
            panel_instances_.Add(panel);
        }
        //scrollbar_.numberOfSteps = choices_.Count;
    }

    public void ChangeSelection(int choice)
    {
        current_choice_ = choice;

        //scroll appropriately and move the pointer
        scrollbar_.value = 1f - ((float)choice / ((float)choices_.Count - 1f));
        //AdjustPointer();

        //sound_manager_.PlaySound(pointer_move_sound_);
    }


    public void SubmitChoice()
    {

    }
}
