using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : MonoBehaviour
{
    public GridGenerator GridData;
    private List<Vector3> visitedNodes;
    private Queue<Vector3> queue;
    // Keep track of visited notes + which nodes did we get from
    // Necessary later for building the path
    IDictionary<Vector3, Vector3> nodeParents;
    // Start is called before the first frame update
    void Start()
    {
        visitedNodes = new List<Vector3>();
        queue = new Queue<Vector3>();
        nodeParents = new Dictionary<Vector3, Vector3>();
    }

    public void StartBFS()
    {
        // Necessary when generating a new grid and starting a new search
        ClearLists();

        uint nodeVisitCount = 0;
        float timeNow = Time.realtimeSinceStartup;

        queue.Enqueue(GridData.StartNode);
        visitedNodes.Add(GridData.StartNode);

        while (queue.Count != 0)
        {
            var currentNode = queue.Dequeue();
            nodeVisitCount++;

            if (currentNode == GridData.GoalNode)
            {
                print("BFS time: " + (Time.realtimeSinceStartup - timeNow).ToString());
                print(string.Format("BFS visits: {0} ({1:F2}%)", nodeVisitCount, (nodeVisitCount / (double)GridData.WalkableCells.Count) * 100));
                GridData.BuildPath(nodeParents);
                return;
            }

            var nearbyNodes = GridData.GetWalkableNodes(currentNode);
            foreach (var item in nearbyNodes)
            {
                if(!visitedNodes.Contains(item))
                {
                    queue.Enqueue(item);
                    visitedNodes.Add(item);
                    nodeParents.Add(item, currentNode);
                }
            }
        }

        GridData.BuildPath(nodeParents);
    }

    private void ClearLists()
    {
        visitedNodes.Clear();
        queue.Clear();
        nodeParents.Clear();
    }
}
