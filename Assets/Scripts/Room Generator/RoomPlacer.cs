using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacer : MonoBehaviour
{

    public GameObject StartRoom;

    public int steps;

    public List<Room> registeredRooms = new List<Room>();

    public bool ready;

    private void Awake()
    {
        StartRoom = Instantiate(StartRoom);
        StartRoom.GetComponent<Room>().currentStep = steps;
    }
}
