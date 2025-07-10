using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CutsceneEvent))]
public class CutsceneEventDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 2; // type + delay (always shown)

        CutsceneEventType type = (CutsceneEventType)property.FindPropertyRelative("type").enumValueIndex;

        switch (type)
        {
            case CutsceneEventType.PanToLocation:
                lines += 2; // position + floatParam1 (speed)
                break;
            case CutsceneEventType.CameraShake:
                lines += 2; // floatParam1 (intensity), floatParam2 (duration)
                break;
            case CutsceneEventType.UnitJump:
                lines += 2; // unitName, floatParam1 (num jumps)
                break;
        }

        return lines * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float y = position.y;

        SerializedProperty typeProp = property.FindPropertyRelative("type");
        SerializedProperty positionProp = property.FindPropertyRelative("position");
        SerializedProperty float1Prop = property.FindPropertyRelative("floatParam1");
        SerializedProperty float2Prop = property.FindPropertyRelative("floatParam2");
        SerializedProperty delayProp = property.FindPropertyRelative("delay");
        SerializedProperty unitProp = property.FindPropertyRelative("unitName");

        // Draw Event Type
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), typeProp);
        y += lineHeight;

        // Draw Delay
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), delayProp);
        y += lineHeight;

        CutsceneEventType type = (CutsceneEventType)typeProp.enumValueIndex;

        switch (type)
        {
            case CutsceneEventType.PanToLocation:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), positionProp, new GUIContent("Target Position"));
                y += lineHeight;
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float1Prop, new GUIContent("Speed"));
                break;

            case CutsceneEventType.CameraShake:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float1Prop, new GUIContent("Intensity"));
                y += lineHeight;
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float2Prop, new GUIContent("Duration"));
                break;

            case CutsceneEventType.UnitJump:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), unitProp, new GUIContent("Target Unit"));
                y += lineHeight;
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float1Prop, new GUIContent("Number of Jumps"));
                break;
        }

        EditorGUI.EndProperty();
    }
}
