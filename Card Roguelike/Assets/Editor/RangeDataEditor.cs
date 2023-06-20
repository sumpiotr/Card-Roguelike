using Actions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

[CustomPropertyDrawer(typeof(RangeData))]
public class RangeDataEditor : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // The 6 comes from extra spacing between the fields (2px each)
        //return height;
        return EditorGUI.GetPropertyHeight(property, null, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
       EditorGUI.BeginProperty(position, label, property);

        //EditorGUI.LabelField(position, label);


        SerializedProperty rangeTypeProperty = property.FindPropertyRelative("rangeType");
        RangeType rangeType = (RangeType)rangeTypeProperty.enumValueIndex;

        Rect foldoutRect = new Rect(position.x, position.y, position.width, 18);
        Rect rangeTypePosition = new Rect(position.x, position.y + 18, position.width, 16);
        Rect position1 = new Rect(position.x, position.y + 36, position.width, 16);
        Rect position2 = new Rect(position.x, position.y + 54, position.width, 16);
        Rect position3 = new Rect(position.x, position.y + 72, position.width, 16);
        Rect position4 = new Rect(position.x, position.y + 90, position.width, 16);

        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);


        //type
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(rangeTypePosition, rangeTypeProperty);
            switch (rangeType)
            {
                case RangeType.Owner:
                    break;
                case RangeType.Previous:
                    break;
                case RangeType.Area:
                    EditorGUI.PropertyField(position1, property.FindPropertyRelative("minRange"));
                    EditorGUI.PropertyField(position2, property.FindPropertyRelative("maxRange"));
                    EditorGUI.PropertyField(position3, property.FindPropertyRelative("source"));
                    EditorGUI.PropertyField(position4, property.FindPropertyRelative("multipleTargets"));
                    break;
                case RangeType.Line:
                    EditorGUI.PropertyField(position1, property.FindPropertyRelative("minRange"));
                    EditorGUI.PropertyField(position2, property.FindPropertyRelative("maxRange"));
                    break;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}
