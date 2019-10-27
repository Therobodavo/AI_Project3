using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : System.IComparable
{
    public bool isObstacle = true;
    public Vector3 position = Vector3.zero;
    public int x = 0;
    public int y = 0;
    public int g = 0;
    public int h = 0;
    public int f = 0;
    public Node parent;

    public int CompareTo(object obj)
    {
        Node newNode = obj as Node;
        if (newNode.f > this.f)
        {
            return -1;
        }
        else if (newNode.f < this.f)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}