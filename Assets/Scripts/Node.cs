using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    [SerializeField] int id;
    [SerializeField] int x;
    [SerializeField] int y;
    [SerializeField] int z;
    [SerializeField] float posX;
    [SerializeField] float posY;
    [SerializeField] float posZ;
    [SerializeField] Node up;
    [SerializeField] Node right;
    [SerializeField] Node left;
    [SerializeField] Node down;
    [SerializeField] GridItem gridItem;
    [SerializeField] Tray tray;
    [SerializeField] GridItemType gridType;
    public int Id { get => id; set => id = value; }
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public int Z { get => z; set => z = value; }
    public float PosX { get => posX; set => posX = value; }
    public float PosY { get => posY; set => posY = value; }
    public float PosZ { get => posZ; set => posZ = value; }
    public Node Up { get => up; set => up = value; }
    public Node Right { get => right; set => right = value; }
    public Node Left { get => left; set => left = value; }
    public Node Down { get => down; set => down = value; }
    public GridItem GridItem { get => gridItem; set => gridItem = value; }
    public Tray Tray { get => tray; set => tray = value; }
    public GridItemType GridType { get => gridType; set => gridType = value; }
}
