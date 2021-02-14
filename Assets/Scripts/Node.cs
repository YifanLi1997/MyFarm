using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    bool walkable;
    Vector3 worldPos;
    int gridX, gridY, movementPenalty;

    int gCost, hCost;

    public int FCost {  get {   return gCost + hCost; } }
    public bool Walkable { get { return walkable; } set { walkable = value; } }
    public Vector3 WorldPos { get { return worldPos; } set { worldPos = value; } }
    public int MovementPenalty { get { return movementPenalty; } set { movementPenalty = value; } }

    public Node(bool _walkable, Vector3 _worldPos, int _x, int _y, int _penalty)
    {
        walkable = _walkable;
        WorldPos = _worldPos;
        gridX = _x;
        gridY = _y;
        MovementPenalty = _penalty;
    }

    
}
