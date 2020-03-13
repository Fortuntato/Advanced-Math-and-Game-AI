using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int Width;
    public int Height;
    public int NumberOfObstacles;
    public GameObject Player;
    public GameObject Goal;
    public GameObject Obstacle;
    [HideInInspector]
    public List<Vector3> Obstacles;
    [HideInInspector]
    public List<Vector3> WalkableCells;
    [HideInInspector]
    public Vector3 StartNode;
    [HideInInspector]
    public Vector3 GoalNode;
    [Header("Debugging")]
    public GameObject WalkableTile;
    public GameObject PathTile;

    // Start is called before the first frame update
    void Start()
    {
        Obstacles = new List<Vector3>();
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        ClearLists();

        // Place Obstacles
        int obstaclesToCreate = NumberOfObstacles;
        while (obstaclesToCreate > 0)
        {
            var obstacle = CreateAtRandomPosition(Obstacle, 0.5f);
            if (obstacle != null)
            {
                Obstacles.Add(obstacle.transform.position);
                obstaclesToCreate--;
            }
        }

        // Place Walkable tiles
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (!IsCellOccupied(new Vector3(x, 0, z)))
                {
                    var tile = Instantiate(WalkableTile, new Vector3(x, 0.05f, z), Quaternion.identity, transform);
                    WalkableCells.Add(new Vector3(x, 0, z));
                }
            }
        }

        // Place Player and Goal
        GameObject startGameObject;
        while ((startGameObject = CreateAtRandomPosition(Player, 0f)) == null);
        StartNode = startGameObject.transform.position;

        GameObject goalGameObject;
        while ((goalGameObject = CreateAtRandomPosition(Goal, 0f)) == null);
        GoalNode = goalGameObject.transform.position;
    }

    private void ClearLists()
    {
        DeleteAllChildren(transform);
        Obstacles.Clear();
        WalkableCells.Clear();
    }

    public List<Vector3> GetNearbyNodes(Vector3 currentNode)
    {
        var result = new List<Vector3>();
        for (int z = -1; z <= 1; z++)
        {
            for (int x = -1; x <= 1; x++)
            {
                // Check if the currentNode is at the edge
                if (currentNode.x + x < 0 || currentNode.x + x >= Width)
                    continue;
                if (currentNode.z + z < 0 || currentNode.z + z >= Height)
                    continue;
                // Don't consider the currentNode
                if (x == 0 && z == 0)
                    continue;

                var node = new Vector3(currentNode.x + x, 0, currentNode.z + z);
                if(!IsCellOccupied(node))
                {
                    result.Add(node);
                }
            }
        }
        return result;
    }

    public IList<Vector3> GetWalkableNodes(Vector3 curr)
    {

        IList<Vector3> walkableNodes = new List<Vector3>();

        IList<Vector3> possibleNodes = new List<Vector3>() {
            new Vector3 (curr.x + 1, 0, curr.z),
            new Vector3 (curr.x - 1, 0, curr.z),
            new Vector3 (curr.x, 0, curr.z + 1),
            new Vector3 (curr.x, 0, curr.z - 1),
            new Vector3 (curr.x + 1, 0, curr.z + 1),
            new Vector3 (curr.x + 1, 0, curr.z - 1),
            new Vector3 (curr.x - 1, 0, curr.z + 1),
            new Vector3 (curr.x - 1, 0, curr.z - 1)
        };

        foreach (Vector3 node in possibleNodes)
        {
            if (!IsCellOccupied(node) && (node.x >= 0 && node.x <= Width - 1) && (node.z >= 0 && node.z <= Height - 1))
            {
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }

    public void BuildPath(IDictionary<Vector3, Vector3> nodeParents)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curr = GoalNode;
        while (curr != StartNode)
        {
            path.Add(curr);
            curr = nodeParents[curr];
        }

        ShowPath(path);
    }
    private void ShowPath(List<Vector3> nodes)
    {
        for (int i = nodes.Count - 1; i > 0; i--)
        {
            var newGameObject = Instantiate(PathTile, new Vector3(nodes[i].x, 0.1f, nodes[i].z), Quaternion.identity, transform);
        }
    }

    private GameObject CreateAtRandomPosition(GameObject prefab, float PositionY)
    {
        var positionX = UnityEngine.Random.Range(0, Width);
        var positionZ = UnityEngine.Random.Range(0, Height);

        if (IsCellOccupied(new Vector3(positionX, 0, positionZ)))
        {
            Debug.Log("CELL OCCUPIED - Recreating: " + prefab.name);
            return null;
        }
        else
        {
            var newGameObject = Instantiate(prefab, new Vector3(positionX, PositionY, positionZ), Quaternion.identity, transform);
            return newGameObject;
        }
    }

    private void DeleteAllChildren(Transform parentGameObject)
    {
        foreach (Transform item in parentGameObject)
        {
            Destroy(item.gameObject);
        }
    }

    private bool IsCellOccupied(Vector3 position)
    {
        foreach (var item in Obstacles)
        {
            if (item.x == position.x && item.z == position.z)
            {
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
