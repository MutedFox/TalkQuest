using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogLine
{
    //Returns the string that makes up the text of the dialog line
    string GetLineBody();

    //Returns other information associated with this line of dialog such as the speaker's name etc.
    //TODO: this may be changed to a different/better format
    List<string> GetData();
}
