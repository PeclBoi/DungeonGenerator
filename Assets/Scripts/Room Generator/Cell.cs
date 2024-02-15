using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour 
{
    public Vector3 position;
    public CellTag zone;
    public CellSideTag side;

    public Vector2 Size;

    //public Cell(Vector3 position, CellTag zone, CellSideTag side)
    //{
    //    this.position = position;
    //    this.zone = zone;
    //    this.side = side;
    //}


    public bool IsFree()
    {
        if (Physics.BoxCast(position, new Vector3(Size.x, 0, Size.y) / 2, Vector3.up, out RaycastHit hit, transform.rotation, 5f))
        {
            return false;
        }

        return true;
    }

}

public enum CellSideTag
{
    Forward,
    Backward,
    Left,
    Right,
    None
}

public enum CellTag
{
    Inside,
    Wall,
    Taken
}