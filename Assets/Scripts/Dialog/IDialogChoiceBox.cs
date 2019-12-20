using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogChoiceBox
{
    void Initialise(List<string> choices);

    void ChangeSelection(int choice);

    void SubmitChoice();
}
