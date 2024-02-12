using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DoorOrientation
{
    North,
    East,
    South,
    West,
}

public class Door : MonoBehaviour
{

    public LayerMask mask;

    public List<GameObject> possibleRooms;

    public bool IsConnected;

    [SerializeField] private DoorOrientation orientation;

    public Room GetRoom()
    {
        if (!possibleRooms.Any()) { return null; }

        int randomIdx = Random.Range(0, possibleRooms.Count);

        var selectedRoom = possibleRooms[randomIdx];

        var roomInstance = Instantiate(selectedRoom);
        possibleRooms.Remove(selectedRoom);
        return roomInstance.GetComponent<Room>();
    }

    public Vector2 Orientation
    {
        get
        {
            return GetDirectionVector();
        }
    }

    private Vector2 GetDirectionVector()
    {

        Vector2 North = new(0, 1);
        Vector2 East = new(1, 0);
        Vector2 South = new(0, -1);
        Vector2 West = new(-1, 0);

        Vector2 oriantationVector;

        switch (orientation)
        {
            case DoorOrientation.North:
                oriantationVector = North;
                break;
            case DoorOrientation.East:
                oriantationVector = East;
                break;
            case DoorOrientation.South:
                oriantationVector = South;
                break;
            case DoorOrientation.West:
                oriantationVector = West;
                break;
            default:
                oriantationVector = Vector2.zero;
                break;
        }

        return oriantationVector;
    }
}