using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Flags]
public enum AdjacentPos
{
    Nothing = 0,
    UpperLeft = 1,
    Up = 2,
    UpperRight = 4,
    Right = 8,
    LowerRight = 16,
    Down = 32,
    LowerLeft = 64,
    Left = 128,
}

[Serializable]
public struct AdjacentTile
{
    public RoomTile tile;
    public AdjacentPos AdjacentPos;
}

[CreateAssetMenu(menuName = "Room Block")]
public class RoomTile : ScriptableObject
{
    public GameObject Block;
    public List<AdjacentTile> AdjacentTiles;
    private List<AdjacentTile> possibleTiles;

    public bool IsTileLoaded = false;

    public Vector2 gridPos;

    public Vector3 TilePos;

    public RoomTile GetAdjacentRoomTile(AdjacentPos position)
    {

        if(position == AdjacentPos.Nothing) { return null; }

        //possibleTiles = AdjacentTiles.Where(a => (AdjacentPos & position) > 0).ToList();

        if (!possibleTiles.Any()) { return null; }

        return possibleTiles[UnityEngine.Random.Range(0, possibleTiles.Count())].tile;
    }



    //public AdjacentPos GetDirectionWithLeastEntropy()
    //{
    //    var x = AdjacentTiles.Min(x => x.AdjacentPos);
    //    return x;
    //}

    public int GetEntropy()
    {
        return possibleTiles.Count;
    }

    public void InstantiateTile(GameObject obj)
    {
        //IsTileLoaded = true;

        var tile = Instantiate(obj);
        tile.transform.position = TilePos;

    }
}
