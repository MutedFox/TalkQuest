using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This interface should be extended by any script that is to be called from a line of dialog
public interface IExecutedFromDialog
{
    void RunFromDialog();
}
