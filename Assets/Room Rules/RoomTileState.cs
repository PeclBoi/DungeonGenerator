using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomTileState : MonoBehaviour
{

    public bool IsTileLoaded;

    public RoomTile roomTile;
    public List<AdjacentTile> ruleSet = new List<AdjacentTile>();


    public void LoadTile(RoomTile tile)
    {
        IsTileLoaded = true;
        roomTile = tile;
        Instantiate(tile.Block, transform);
    }

    public void CollapsePossibleTiles(List<AdjacentTile> adjacentTiles)
    {
        if (!ruleSet.Any()) { ruleSet = adjacentTiles; }
        //TODO: Remove Impossible Combinations
        else { ruleSet = ruleSet.Intersect(adjacentTiles).ToList(); }

    }

    public void LoadPossibleTile()
    {
        IsTileLoaded = true;
        roomTile = ruleSet[Random.Range(0, ruleSet.Count)].tile;
        Instantiate(roomTile.Block, transform);
    }
}
