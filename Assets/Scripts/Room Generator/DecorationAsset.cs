using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Decor")]
public class DecorationAsset : ScriptableObject
{
    public GameObject prefab;
    public CellTag zone;

    public bool IsStuck()
    {
        //TODO
        return false;
    }
}