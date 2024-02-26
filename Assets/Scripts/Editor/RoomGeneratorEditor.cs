using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(RoomPlacer)), CanEditMultipleObjects]
public class RoomGeneratorEditor : Editor
{
    public List<Room> registeredRooms = new List<Room>();


    SerializedObject so;

    SerializedProperty startRoomPrefab;
    SerializedProperty endRoomPrefab;
    SerializedProperty keyPrefab;
    SerializedProperty agentPrefab;
    SerializedProperty navMeshSurface;
    SerializedProperty propRegisteredRoom;
    SerializedProperty propSteps;
    SerializedProperty propRoomCount;

    private bool IsRoomsGenrated = false;

    [SerializeField] int generationEndingCondition;

    public RoomPlacer _target;

    private void OnEnable()
    {
        so = serializedObject;

        startRoomPrefab = so.FindProperty("StartRoom");
        endRoomPrefab = so.FindProperty("EndRoom");
        keyPrefab = so.FindProperty("KeyPrefab");
        agentPrefab = so.FindProperty("navMeshSurface");
        navMeshSurface = so.FindProperty("agent");
        propRegisteredRoom = so.FindProperty("registeredRooms");
        propSteps = so.FindProperty("steps");
        propRoomCount = so.FindProperty("numberOfRooms");

        _target = (RoomPlacer)target;
    }

    public override void OnInspectorGUI()
    {
        so.Update();

        EditorGUILayout.PropertyField(startRoomPrefab);
        EditorGUILayout.PropertyField(endRoomPrefab);
        EditorGUILayout.PropertyField(keyPrefab);
        EditorGUILayout.PropertyField(agentPrefab);
        EditorGUILayout.PropertyField(navMeshSurface);
        EditorGUILayout.PropertyField(propRegisteredRoom);

        GUILayout.Label("End Generation based on:");
        generationEndingCondition = GUILayout.Toolbar(generationEndingCondition, new string[] { "Interations", "Rooms" });

        if (generationEndingCondition == 0)
        {
            EditorGUILayout.PropertyField(propSteps);
            _target.endingCondition = EndingCondition.Interation;
        }

        if (generationEndingCondition == 1)
        {
            EditorGUILayout.PropertyField(propRoomCount);
            _target.endingCondition = EndingCondition.RoomCount;
        }

        if (!IsRoomsGenrated)
        {
            if (GUILayout.Button("Generate Rooms"))
            {
                _target.PlaceRooms();
                IsRoomsGenrated = true;
            }
        }

        if (GUILayout.Button("Generate Room Content"))
        {
            _target.PlaceAssetsInRooms();
        }

        if (GUILayout.Button("Place NPCs"))
        {
            _target.PlaceNPCsInRooms();
        }

        if (GUILayout.Button("Select End Room"))
        {
            _target.SelectEndRoom();
        }

        if (GUILayout.Button("Check If Dungeon is Winnable"))
        {
            _target.CheckIfWinnable();
        }

        so.ApplyModifiedProperties();

    }

}
