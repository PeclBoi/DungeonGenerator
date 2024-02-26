using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public enum EndingCondition
{
    Interation,
    RoomCount
}

public class RoomPlacer : MonoBehaviour
{

    public GameObject StartRoom;
    public GameObject EndRoom;
    public GameObject KeyPrefab;

    public bool ready;

    public int steps;
    public int numberOfRooms;

    public List<Room> registeredRooms = new List<Room>();

    public NavMeshSurface navMeshSurface;
    public NavMeshAgent agent;

    private Transform _spawnPos;
    private Transform _keyPos;
    private bool _lastStepReached;

    public EndingCondition endingCondition;
    private GameObject roomsContainer;
    public static RoomPlacer Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaceRooms()
    {
        Room.OnRegisterRoom += RegisterRoom;
        roomsContainer = new GameObject("Dungeon");
        Instantiate(StartRoom, roomsContainer.transform);
        StartRoom.GetComponent<Room>().currentStep = steps;
        _spawnPos = StartRoom.GetComponent<Spawn>().spawnPosition;
    }

    public void PlaceAssetsInRooms()
    {
        foreach (var registeredRoom in registeredRooms)
        {
            registeredRoom.FillRoomWithAssets();
        }
    }

    public void PlaceNPCsInRooms()
    {
        foreach (var registeredRoom in registeredRooms)
        {
            registeredRoom.PlaceNPCsInRoom();
        }
    }

    private bool IsEndingConditionFulfilled()
    {
        if (endingCondition == EndingCondition.Interation)
        {
            return _lastStepReached;
        }
        else if (endingCondition == EndingCondition.RoomCount)
        {
            return registeredRooms.Count == numberOfRooms;
        }

        return false;
    }

    private void RegisterRoom(Room newRoom)
    {
        if (newRoom.currentStep == -1)
        {
            _lastStepReached = true;
        }

        if (IsEndingConditionFulfilled())
        {
            Destroy(newRoom.gameObject);
            StartCoroutine(CloseUnconnectedDoors());
            return;
        }


        newRoom.transform.SetParent(roomsContainer.transform);
        registeredRooms.Add(newRoom);
        return;
    }

    public IEnumerator CloseUnconnectedDoors()
    {
        Debug.Log("Close Doors");
        yield return new WaitForSeconds(1f);
        foreach (var room in registeredRooms)
        {
            room.CloseOffUnconnectedRooms();
        }
    }

    public void DeregisterRoom(Room destroyedRoom)
    {
        registeredRooms.Remove(destroyedRoom);
    }


    public void SelectEndRoom()
    {
        var randomIndex = Random.Range(1, registeredRooms.Count);
        registeredRooms[randomIndex].ChangeToEndRoom();
        _keyPos = registeredRooms[randomIndex].keyPos;
        Physics.Raycast(_keyPos.position, -Vector3.up, out RaycastHit hit, 5);
        Debug.Log(hit.point);
        _keyPos.position = hit.point;
    }

    public void CheckIfWinnable()
    {
        Debug.Log("Checking if possible...");
        navMeshSurface.BuildNavMesh();
        var agentInstance = Instantiate(agent);
        var pathChecker = new PathChecker(agentInstance.GetComponent<NavMeshAgent>());

        if (pathChecker.IsPathClear(_spawnPos.position, _keyPos.position))
        {
            Debug.Log("Path Is Clear");
        }
        else
        {
            Debug.Log("Path Is Blocked");
        }


    }
}
