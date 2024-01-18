using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public Block[] tileOptions;

    public void CreateCell(bool collapseState, Block[] tiles)
    {
        collapsed = collapseState;
        tileOptions = tiles;
    }

    public void RecreateCell(Block[] tiles)
    {
        tileOptions = tiles;
    }
}
