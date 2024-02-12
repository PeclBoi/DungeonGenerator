using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

public class Room : MonoBehaviour
{

    public Door[] doors;

    [Tooltip("Last Object to be used when no other room fits")]
    public GameObject Wall;

    public Validator validator;
    public bool IsSpawned;

    public RoomPlacer placer;

    public int currentStep;

    private void Start()
    {
        placer = GameObject.Find("RoomPlacer").GetComponent<RoomPlacer>();
    }

    public void Update()
    {
        if (IsSpawned) { return; }

        if (currentStep > 0 && placer.ready)
        {
            StartCoroutine(AddAdjecentRooms());
            IsSpawned = true;
        }

    }

    public IEnumerator CloseOffUnconnectedRooms()
    {
        yield return new WaitUntil(() => validator.IsValid);

        foreach (var door in doors.Where(d => !d.IsConnected))
        {
            PlaceWallAtDoor(Wall, door);
            door.IsConnected = true;
        }
    }

    public Door GetRandomDoor()
    {
        var openDoors = doors.Where(d => !d.IsConnected).ToList();

        if (!openDoors.Any()) { return null; }

        int rngIndex = Random.Range(0, openDoors.Count);

        var door = doors[rngIndex];
        return door;
    }

    private void PlaceRoomAtDoor(Door doorPosition, Door otherDoorPosition)
    {

        int counter = 3;
        while (Vector3.Angle(doorPosition.transform.forward, otherDoorPosition.transform.forward) < 179 || Vector3.Angle(doorPosition.transform.forward, otherDoorPosition.transform.forward) > 181)
        {
            otherDoorPosition.gameObject.transform.root.Rotate(0, 90, 0);
            if (counter-- < 0)
            {
                break;
            }
        }
        var offset = otherDoorPosition.transform.position - otherDoorPosition.transform.root.position;

        otherDoorPosition.transform.root.position = doorPosition.transform.position - offset;
    }

    public IEnumerator AddAdjecentRooms()
    {
        placer.ready = false;
        yield return new WaitUntil(() => validator.IsValid);


        foreach (var door in doors.Where(d => !d.IsConnected))
        {
            Room room;
            Door otherDoor = null;

            do
            {
                room = door.GetRoom();

                if (room == null)
                {
                    PlaceWallAtDoor(Wall, door);
                    break;
                }

                otherDoor = room.GetRandomDoor();
                PlaceRoomAtDoor(door, otherDoor);
                yield return new WaitUntil(() => room.validator._isStateSet);

                if (room.validator.IsValid)
                {
                    room.currentStep = currentStep - 1;

                    if (room.currentStep == 0)
                    {
                        StartCoroutine(room.CloseOffUnconnectedRooms());
                    }

                }

            } while (!room.validator.IsValid);

            otherDoor.IsConnected = true;
            door.IsConnected = true;
        }

        placer.ready = true;
    }

    private void PlaceWallAtDoor(GameObject wall, Door door)
    {
        wall = Instantiate(wall, door.transform.position, door.transform.rotation);
        wall.transform.SetParent(door.transform.parent);

        door.gameObject.SetActive(false);

    }
}
