using Actions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ActionData))]
public class ActionDataObjectEditor : PropertyDrawer
{

    SerializedProperty actionType;
    SerializedProperty actionData;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PrefixLabel(position, label);

        EditorGUI.EndProperty();
    }
}
