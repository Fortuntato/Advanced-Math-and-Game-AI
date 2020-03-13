using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public GridGenerator GridData;
    private SimplePriorityQueue<Vector3, int> priorityQueue;
    // Keep track of all visited notes + which node we got from
    // Necessary later for building the path
    Dictionary<Vector3, Vector3> nodeParents;
    // Start is called before the first frame update
    void Start()
    {
        priorityQueue = new SimplePriorityQueue<Vector3, int>();
        nodeParents = new Dictionary<Vector3, Vector3>();
    }

    public void StartAStar()
    {
        // Necessary when generating a new grid and starting a new search
        ClearLists();


        // Represents h(x)
        IDictionary<Vector3, int> heuristicScore = new Dictionary<Vector3, int>();

        // Represents g(x) or the distance from start to node "x" (Same meaning as in Dijkstra's "distances")
        IDictionary<Vector3, int> distanceFromStart = new Dictionary<Vector3, int>();

        foreach (Vector3 vertex in GridData.WalkableCells)
        {
            heuristicScore.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));
            distanceFromStart.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));
        }

        heuristicScore[GridData.StartNode] = DistanceEstimate(GridData.StartNode);
        distanceFromStart[GridData.StartNode] = 0;

        priorityQueue.Enqueue(GridData.StartNode, heuristicScore[GridData.StartNode]);

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
                var currentScore = distanceFromStart[currentNode] + 1; // 1 is the weight

                if (currentScore < distanceFromStart[item])
                {
                    distanceFromStart[item] = currentScore;
                    //nodeParents[item] = currentNode;

                    int hScore = distanceFromStart[item] + DistanceEstimate(item);
                    heuristicScore[item] = hScore;

                    if (!priorityQueue.Contains(item))
                    {
                        priorityQueue.Enqueue(item, currentScore);
                        nodeParents.Add(item, currentNode);
                    }
                }
            }
        }

        GridData.BuildPath(nodeParents);
    }

    int DistanceEstimate(Vector3 node)
    {
        var goal = GridData.Goal.transform.position;
        return (int)Mathf.Sqrt(Mathf.Pow(node.x - goal.x, 2) +
            Mathf.Pow(node.y - goal.y, 2) +
            Mathf.Pow(node.z - goal.z, 2));
    }

    private void ClearLists()
    {
        priorityQueue.Clear();
        nodeParents.Clear();
    }
}
