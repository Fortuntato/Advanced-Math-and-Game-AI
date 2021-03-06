﻿using Priority_Queue;
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
        ClearLists();

        uint nodeVisitCount = 0;
        float timeNow = Time.realtimeSinceStartup;

        // A* tries to minimize f(x) = g(x) + h(x), where g(x) is the distance from the start to node "x" and
        //    h(x) is some heuristic that must be admissible, meaning it never overestimates the cost to the next node.
        //    There are formal logical proofs you can look up that determine how heuristics are and are not admissible.

        IEnumerable<Vector3> validNodes = GridData.WalkableCells;

        // Represents h(x) or the score from whatever heuristic we're using
        IDictionary<Vector3, int> heuristicScore = new Dictionary<Vector3, int>();

        // Represents g(x) or the distance from start to node "x" (Same meaning as in Dijkstra's "distances")
        IDictionary<Vector3, int> distanceFromStart = new Dictionary<Vector3, int>();

        foreach (Vector3 vertex in validNodes)
        {
            heuristicScore.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));
            distanceFromStart.Add(new KeyValuePair<Vector3, int>(vertex, int.MaxValue));
        }

        heuristicScore[GridData.StartNode] = DistanceEstimate(GridData.StartNode);
        distanceFromStart[GridData.StartNode] = 0;

        // The item dequeued from a priority queue will always be the one with the lowest int value
        //    In this case we will input nodes with their calculated distances from the start g(x),
        //    so we will always take the node with the lowest distance from the queue.
        SimplePriorityQueue<Vector3, int> priorityQueue = new SimplePriorityQueue<Vector3, int>();
        priorityQueue.Enqueue(GridData.StartNode, heuristicScore[GridData.StartNode]);

        while (priorityQueue.Count > 0)
        {
            // Get the node with the least distance from the start
            Vector3 curr = priorityQueue.Dequeue();
            nodeVisitCount++;

            // If our current node is the goal then stop
            if (curr == GridData.GoalNode)
            {
                print("A*" + " time: " + (Time.realtimeSinceStartup - timeNow).ToString());
                print(string.Format("A* visits: {0} ({1:F2}%)", nodeVisitCount, (nodeVisitCount / (double)GridData.WalkableCells.Count) * 100));
                GridData.BuildPath(nodeParents);
                return;
            }

            IList<Vector3> neighbors = GridData.GetWalkableNodes(curr);

            foreach (Vector3 node in neighbors)
            {
                // Get the distance so far, add it to the distance to the neighbor
                int currScore = distanceFromStart[curr] + 1;

                // If our distance to this neighbor is LESS than another calculated shortest path
                //    to this neighbor, set a new node parent and update the scores as our current
                //    best for the path so far.
                if (currScore < distanceFromStart[node])
                {
                    nodeParents[node] = curr;
                    distanceFromStart[node] = currScore;

                    int hScore = distanceFromStart[node] + DistanceEstimate(node);
                    heuristicScore[node] = hScore;

                    // If this node isn't already in the queue, make sure to add it. Since the
                    //    algorithm is always looking for the smallest distance, any existing entry
                    //    would have a higher priority anyway.
                    if (!priorityQueue.Contains(node))
                    {
                        priorityQueue.Enqueue(node, hScore);
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
