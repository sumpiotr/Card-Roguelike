using Actions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ActionData))]

public class ActionDataEditor : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, null, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty actionTypeProperty = property.FindPropertyRelative("type");
        ActionType actionType = (ActionType)actionTypeProperty.enumValueIndex;

        Rect foldoutRect = new Rect(position.x, position.y, position.width, 18);
        Rect rangeTypePosition = new Rect(position.x, position.y + 18, position.width, 16);
        Rect position1 = new Rect(position.x, position.y + 36, position.width, 16);
        Rect position2 = new Rect(position.x, position.y + 54, position.width, 16);
        Rect position3 = new Rect(position.x, position.y + 72, position.width, 16);

        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);


        //type
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(rangeTypePosition, actionTypeProperty);
            switch (actionType)
            {
                case ActionType.Attack:
                    EditorGUI.PropertyField(position1, property.FindPropertyRelative("value"));
                    EditorGUI.PropertyField(position2, property.FindPropertyRelative("range"));
                    break;
                case ActionType.ChooseAttackTarget:
                    EditorGUI.PropertyField(position1, property.FindPropertyRelative("range"));
                    break;
                case ActionType.Buff:
                    EditorGUI.PropertyField(position1, property.FindPropertyRelative("value"));
                    EditorGUI.PropertyField(position2, property.FindPropertyRelative("stat"));
                    EditorGUI.PropertyField(position3, property.FindPropertyRelative("range"));
                    break;
                case ActionType.Move:
                    EditorGUI.PropertyField(position1, property.FindPropertyRelative("value"));
                    EditorGUI.PropertyField(position2, property.FindPropertyRelative("range"));
                    break;
                case ActionType.ChooseMoveTarget:
                    EditorGUI.PropertyField(position1, property.FindPropertyRelative("range"));
                    break;
                case ActionType.Push:
                    EditorGUI.PropertyField(position1, property.FindPropertyRelative("value"));
                    EditorGUI.PropertyField(position2, property.FindPropertyRelative("range"));
                    break;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}

