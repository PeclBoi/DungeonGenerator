using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static Unity.VisualScripting.Metadata;
using static UnityEngine.GraphicsBuffer;

public class DungeonRoom : MonoBehaviour
{

    public GameObject openDoor;

    public GameObject otherRoom;

    // Start is called before the first frame update
    void Start()
    {
        AppendRoom();
    }



    public void AppendRoom()
    {
        var room = Instantiate(otherRoom);

        openDoor.transform.rotation = room.transform.rotation * Quaternion.Inverse(Quaternion.Inverse(openDoor.transform.rotation) * openDoor.transform.rotation);
        room.transform.position = new Vector3(room.transform.position.x, openDoor.transform.position.y - openDoor.transform.localPosition.y, openDoor.transform.position.z);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
