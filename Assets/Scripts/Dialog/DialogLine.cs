using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogLine : IDialogLine
{
    //The main content of this line of dialog (i.e. the actual dialog line itself)
    private string text_body_;

    //The name of the person talking (can be left blank)
    private string speaker_name_;

    //name of image to use for this line of dialog (character portrait)
    private string portrait_name_;

    public DialogLine(string body, string speaker_name, string portrait_name)
    {
        text_body_ = body;
        speaker_name_ = speaker_name;
        portrait_name_ = portrait_name;
    }

    public string GetLineBody()
    {
        return text_body_;
    }

    public List<string> GetData()
    {
        return new List<string>() { speaker_name_, portrait_name_ };
    }
}
