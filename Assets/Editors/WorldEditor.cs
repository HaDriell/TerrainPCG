using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
[CanEditMultipleObjects]
public class WorldEditor : Editor
{
    SerializedProperty width;
    SerializedProperty height;
    SerializedProperty depth;

    SerializedProperty snowHeight;
    SerializedProperty grassValue;
    SerializedProperty rockValue;

    SerializedProperty grassColor;
    SerializedProperty rockColor;
    SerializedProperty snowColor;


    private void OnEnable()
    {
        width       = serializedObject.FindProperty("width");
        height      = serializedObject.FindProperty("height");
        depth       = serializedObject.FindProperty("depth");

        snowHeight  = serializedObject.FindProperty("snowHeight");
        grassValue  = serializedObject.FindProperty("grassValue");
        rockValue   = serializedObject.FindProperty("rockValue");

        grassColor  = serializedObject.FindProperty("grassColor");
        rockColor   = serializedObject.FindProperty("rockColor");
        snowColor   = serializedObject.FindProperty("snowColor");
    }

    public override void OnInspectorGUI()
    {
        World world = (World)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(height);
        EditorGUILayout.PropertyField(depth);

        EditorGUILayout.PropertyField(snowHeight);
        EditorGUILayout.PropertyField(grassValue);
        EditorGUILayout.PropertyField(rockValue);

        EditorGUILayout.PropertyField(grassColor);
        EditorGUILayout.PropertyField(rockColor);
        EditorGUILayout.PropertyField(snowColor);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Compute Operators"))
        {
            world.Compute();
        }
    }
}
