using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3 position;
    public CellTag zone;

    public Vector2 Size;
    Quaternion transformRotation;

    public Cell(Vector3 position, CellTag zone, Quaternion transformRotation)
    {
        this.position = position;
        this.zone = zone;
        this.transformRotation = transformRotation;
    }


    public bool IsFree()
    {
        if (Physics.BoxCast(position, new Vector3(Size.x, 0, Size.y) / 2, Vector3.up, out RaycastHit hit, transformRotation, 5f))
        {
            return false;
        }

        return true;
    }
}

public enum CellTag
{
    Inside,
    Wall,
    Taken
}