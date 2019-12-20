using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionHandler
{
    bool EvaluateFlag(string name, string value);

    void UpdateFlag(string name, string value);
}
