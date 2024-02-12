using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{

    public Transform doorPosition, otherDoorPosition;


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var angleX = Vector3.Angle(doorPosition.right, otherDoorPosition.right);
            angleX = 180 - angleX;

            doorPosition.root.Rotate(0, angleX, 0);

            var angleZ = Vector3.Angle(doorPosition.forward, otherDoorPosition.forward);
            if (angleZ < 0.2)
            {
                doorPosition.root.Rotate(0, 180, 0);
            }

            var offset = doorPosition.position - doorPosition.root.position;

            doorPosition.root.position = otherDoorPosition.position - offset;
        }
    }
}
