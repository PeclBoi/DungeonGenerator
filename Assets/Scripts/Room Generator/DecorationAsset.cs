using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Decor")]
public class DecorationAsset : ScriptableObject
{
    public GameObject prefab;
    public Vector2 area;
    public CellTag zone;

    [Range(0, 1)]
    public float chances;

    public bool IsStuck()
    {
        //TODO
        return false;
    }
}