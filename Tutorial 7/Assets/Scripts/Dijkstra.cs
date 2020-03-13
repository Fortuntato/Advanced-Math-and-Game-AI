using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public GridGenerator GridData;
    // Keep track of all visited notes + which node we got from
    // Necessary later for building the path
    IDictionary<Vector3, Vector3> nodeParents;
    // Start is called before the first frame update
    void Start()
    {
        nodeParents = new Dictionary<Vector3, Vector3>();
    }

    public void StartDijkstra()
    {
        // Necessary when generating a new grid and starting a new search
        ClearLists();

        uint nodeVisitCount = 0;
        float timeNow = Time.realtimeSinceStartup;

        //A priority queue containing the shortest distance so far from the start to a given node
        IPriorityQueue<Vector3, int> priority = new SimplePriorityQueue<Vector3, int>();

        //A list of all nodes that are walkable, initialized to have infinity distance from start
        IDictionary<Vector3, int> distances = GridData.WalkableCells
            .ToDictionary(x => x, x => int.MaxValue);

        //Our distance from the start to itself is 0
        distances[GridData.StartNode] = 0;
        priority.Enqueue(GridData.StartNode, 0);

        while (priority.Count > 0)
        {

            Vector3 curr = priority.Dequeue();
            nodeVisitCount++;

            if (curr == GridData.GoalNode)
            {
                // If the goal position is the lowest position in the priority queue then there are
                //    no other nodes that could possibly have a shorter path.
                print("Dijkstra: " + distances[GridData.StartNode]);
                print("Dijkstra time: " + (Time.realtimeSinceStartup - timeNow).ToString());
                print(string.Format("Dijkstra visits: {0} ({1:F2}%)", nodeVisitCount, (nodeVisitCount / (double)GridData.WalkableCells.Count) * 100));

                GridData.BuildPath(nodeParents);
                return;
            }

            IList<Vector3> nodes = GridData.GetWalkableNodes(curr);

            //Look at each neighbor to the node
            foreach (Vector3 node in nodes)
            {

                int dist = distances[curr] + 1;

                //If the distance to the parent, PLUS the distance added by the neighbor,
                //is less than the current distance to the neighbor,
                //update the neighbor's paent to curr, update its current best distance
                if (dist < distances[node])
                {
                    distances[node] = dist;

                    if (!priority.Contains(node))
                    {
                        nodeParents.Add(node, curr);
                        priority.Enqueue(node, dist);
                    }
                }
            }
        }

        GridData.BuildPath(nodeParents);
    }

    private void ClearLists()
    {
        nodeParents.Clear();
    }
}
