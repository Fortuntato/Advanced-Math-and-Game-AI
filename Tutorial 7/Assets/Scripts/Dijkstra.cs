using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public GridGenerator GridData;
    private SimplePriorityQueue<Vector3, int> priorityQueue;
    private Dictionary<Vector3, int> distances;
    // Keep track of all visited notes + which node we got from
    // Necessary later for building the path
    Dictionary<Vector3, Vector3> nodeParents;
    // Start is called before the first frame update
    void Start()
    {
        priorityQueue = new SimplePriorityQueue<Vector3, int>();
        nodeParents = new Dictionary<Vector3, Vector3>();
        distances = new Dictionary<Vector3, int>();
    }

    public void StartDijkstra()
    {
        // Necessary when generating a new grid and starting a new search
        ClearLists();

        //GridData.StartNode = new Vector3(0,0,0);
        //GridData.GoalNode = new Vector3(29f, 0, 29f);

        // If you move this line to Start it won't work as WalkableCells is not set yet
        distances = GridData.WalkableCells.ToDictionary(x => x, y => int.MaxValue);

        distances[GridData.StartNode] = 0;
        priorityQueue.Enqueue(GridData.StartNode, 0);

        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Dequeue();

            if(currentNode == GridData.GoalNode)
            {
                GridData.BuildPath(nodeParents);
                return;
            }

            var nearbyNodes = GridData.GetNearbyNodes(currentNode);
            foreach (var item in nearbyNodes)
            {
                var currDistance = distances[currentNode]; // 1 is the weight
                var itemDistance = distances[item];
                if (currDistance < itemDistance)
                {
                    distances[item] = currDistance;
                    //nodeParents[item] = currentNode;
                    if (!priorityQueue.Contains(item))
                    {
                        priorityQueue.Enqueue(item, currDistance);
                        nodeParents.Add(item, currentNode);
                    }
                }
            }
        }

        GridData.BuildPath(nodeParents);
    }

    private void ClearLists()
    {
        distances.Clear();
        priorityQueue.Clear();
        nodeParents.Clear();
    }
}
