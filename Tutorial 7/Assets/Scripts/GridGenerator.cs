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
    private List<Vector3> obstacles;
    [Header("Debugging")]
    public bool ShowTiles;
    public GameObject WalkableTile;
    public GameObject PathTile;

    // Start is called before the first frame update
    void Start()
    {
        obstacles = new List<Vector3>();
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        DeleteAllChildren(transform);
        obstacles.Clear();

        // Place Obstacles
        int obstaclesToCreate = NumberOfObstacles;
        while (obstaclesToCreate > 0)
        {
            var obstacle = CreateAtRandomPosition(Obstacle, 0.5f);
            if(obstacle != null)
            {
                obstacles.Add(obstacle.transform.position);
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
                    Instantiate(WalkableTile, new Vector3(x, 0.05f, z), Quaternion.identity, transform);
                }
            }
        }

        // Place Player and Goal
        while(!CreateAtRandomPosition(Player, 1f));
        while(!CreateAtRandomPosition(Goal, 0.5f));
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
        foreach (var item in obstacles)
        {
            if(item.x == position.x && item.z == position.z)
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
