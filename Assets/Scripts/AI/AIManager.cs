using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    public Transform checkpoints;

    [HideInInspector]
    public bool pathReady;

    int startTarget;
    int currentTarget;
    int addingCounter;

    public List<Transform> targets;

    Vector3[] path;

    void Start()
    {
        pathReady = false;

        SetUpTargetList();
        currentTarget = 1;

        CreatePath();
    }
    
    void SetUpTargetList()
    {
        targets = new List<Transform>();

        for (int i = 0; i < checkpoints.childCount; i++)
            targets.Add(checkpoints.GetChild(i));
    }

    void CreatePath()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            PathRequestController.RequestPath(targets[startTarget].position, targets[currentTarget].position, OnPathFound);

            startTarget++;
            currentTarget++;

            if (currentTarget == targets.Count)
                currentTarget = 0;
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
            AddToPath(newPath);       
    }

    void AddToPath(Vector3[] newPath)
    {
        if (path != null)
        {
            Vector3[] pathBuffer = path;
            path = new Vector3[pathBuffer.Length + newPath.Length];

            for (int i = 0; i < pathBuffer.Length; i++)
                path[i] = pathBuffer[i];

            for (int i = 0; i < newPath.Length; i++)
                path[pathBuffer.Length + i] = newPath[i];
        }
        else
            path = newPath;

        addingCounter++;

        if (addingCounter == targets.Count)
            pathReady = true;
    }

    public Vector3[] _path
    {
        get
        {
            return path;
        }
    }


}
