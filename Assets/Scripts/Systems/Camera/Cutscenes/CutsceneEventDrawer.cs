using UnityEditor;
using UnityEngine;

// This is just to make the scriptable object interface for cutscene events to be easier to work with
[CustomPropertyDrawer(typeof(CutsceneEvent))]
public class CutsceneEventDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 2; // base 2 lines (event type and pre-delay)

        CutsceneEventType type = (CutsceneEventType)property.FindPropertyRelative("type").enumValueIndex;

        switch (type)
        {
            // adds more lines depending on which type and how many params they need
            case CutsceneEventType.PanToLocation:
                lines += 2; // vector3Param (position), floatParam1 (speed)
                break;
            case CutsceneEventType.PanToUnit:
                lines += 2; // stringParam1 (unitName), floatParam1 (speed)
                break;
            case CutsceneEventType.CameraShake:
                lines += 2; // floatParam1 (intensity), floatParam2 (duration)
                break;
            case CutsceneEventType.UnitJump:
                lines += 2; // stringParam1 (unitName), floatParam1 (num jumps)
                break;
            case CutsceneEventType.Wait:
                lines += 1; // floatParam1 (Duration)
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
        SerializedProperty vector3Prop = property.FindPropertyRelative("vector3Param");
        SerializedProperty float1Prop = property.FindPropertyRelative("floatParam1");
        SerializedProperty float2Prop = property.FindPropertyRelative("floatParam2");
        SerializedProperty delayProp = property.FindPropertyRelative("delay");
        SerializedProperty stringParam1Prop = property.FindPropertyRelative("stringParam1");
        SerializedProperty stringParam2Prop = property.FindPropertyRelative("stringParam2");

        // Draw Event Type
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), typeProp);
        y += lineHeight;

        // Draw Delay
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), delayProp);
        y += lineHeight;

        CutsceneEventType type = (CutsceneEventType)typeProp.enumValueIndex;

        switch (type)
        {
            // Draw only needed labels
            case CutsceneEventType.PanToLocation:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), vector3Prop, new GUIContent("Target Position"));
                y += lineHeight;
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float1Prop, new GUIContent("Speed"));
                break;

            case CutsceneEventType.PanToUnit:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), stringParam1Prop, new GUIContent("Target Unit"));
                y += lineHeight;
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float1Prop, new GUIContent("Speed"));
                break;

            case CutsceneEventType.CameraShake:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float1Prop, new GUIContent("Intensity"));
                y += lineHeight;
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float2Prop, new GUIContent("Duration"));
                break;

            case CutsceneEventType.UnitJump:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), stringParam1Prop, new GUIContent("Target Unit"));
                y += lineHeight;
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float1Prop, new GUIContent("Number of Jumps"));
                break;
            case CutsceneEventType.Wait:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), float1Prop, new GUIContent("Duration"));
                y += lineHeight;
                break;
        }

        EditorGUI.EndProperty();
    }
}
