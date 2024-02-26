using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RoomContets")]
public class RoomContent : ScriptableObject
{
    public Decoration[] Decoration;
}

[Serializable]
public struct Decoration
{
    public DecorationAsset DecorationAsset;

    [Range(0,1)]
    public float probability;
}