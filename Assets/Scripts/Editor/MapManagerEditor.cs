using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager))]
[CanEditMultipleObjects]
public class MapManagerEditor : Editor {

    private string[] mapOptions;
    SerializedProperty sceneTilesListProp;
    SerializedProperty mapInfoListProp;
    SerializedProperty mapNameStringProp;

    void OnEnable()
    {
        sceneTilesListProp = serializedObject.FindProperty("sceneTiles");
        mapInfoListProp = serializedObject.FindProperty("mapInfo");
        mapNameStringProp = serializedObject.FindProperty("mapName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MapManager mm = (MapManager)target;
        //DrawDefaultInspector();

        mm.autoGeneration = EditorGUILayout.Toggle("Auto Generation", mm.autoGeneration);
        if (!mm.autoGeneration)
        {
            EditorGUILayout.PropertyField(sceneTilesListProp, true);
            EditorGUILayout.PropertyField(mapNameStringProp);
        }
        else
        {
            EditorGUILayout.PropertyField(mapInfoListProp, true);
            
            mapOptions = new string[mm.mapInfo.Count];
            for (int i = 0; i < mm.mapInfo.Count; i++)
            {
                mapOptions[i] = mm.mapInfo[i].name;
            }
            mm.mapIndex = Mathf.Clamp(EditorGUILayout.Popup("Map Name", mm.mapIndex, mapOptions), 0, mm.mapInfo.Count - 1);
            if (mm.mapInfo.Count > 0)
                mm.mapName = mapOptions[mm.mapIndex];

            mm.size = EditorGUILayout.Vector2IntField("Map Size", mm.size);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
