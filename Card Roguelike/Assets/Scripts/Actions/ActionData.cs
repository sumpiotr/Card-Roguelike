using Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionData : ScriptableObject
{
    public ActionType actionType;
    public BaseActionData actionData;
}

