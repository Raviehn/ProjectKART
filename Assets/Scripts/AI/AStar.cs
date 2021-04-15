using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AStar : MonoBehaviour
{
    PathRequestController requestController;
    Grid grid;

    void Awake()
    {
        requestController = GetComponent<PathRequestController>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 start, Vector3 target)
    {
        StartCoroutine(FindPath(start, target));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromWorldPos(startPos);
        Node targetNode = grid.GetNodeFromWorldPos(targetPos);

        if (startNode.free && targetNode.free)
        {
            Heap<Node> open = new Heap<Node>(grid.MaxSize);
            HashSet<Node> close = new HashSet<Node>();

            open.Add(startNode);

            while (open.Count > 0)
            {
                Node currentNode = open.RemoveFirst();
                close.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.free || close.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) * currentNode.weight;

                    if (newMovementCostToNeighbour < neighbour.gCost || !open.Contains(neighbour))
                    {

                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!open.Contains(neighbour))
                            open.Add(neighbour);
                        else
                            open.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);

        requestController.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = new Vector3[path.Count];

        for (int i = 0; i < path.Count; i++)
            waypoints[path.Count - 1 - i] = path[i].worldPos;

        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();

        for (int i = 1; i < path.Count; i++)
            waypoints.Add(path[i].worldPos);

        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        if (distX > distZ)
            return 14 * distZ + 10 * (distX - distZ);
        else
            return 14 * distX + 10 * (distZ - distX);
    }
}
