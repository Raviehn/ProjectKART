using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public bool free;

    public int weight;
    public int gCost;
    public int hCost;

    public int gridX;
    public int gridZ;

    public Vector3 worldPos;

    public Node parent;

    private int heapIndex;

    public Node(Vector3 _worldPos, bool _free, int _weight, int _gridX, int _gridZ)
    {
        worldPos = _worldPos;
        free = _free;
        weight = _weight;
        gridX = _gridX;
        gridZ = _gridZ;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);

        return -compare;
    }
}