using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMeans : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject Point;
    public GameObject Centroid;
    public int Points;
    public int Centroids;
    private List<GameObject> points;
    private List<GameObject> centroids;
    private List<Color> colors;
    private Dictionary<GameObject, List<GameObject>> clusteredPoints;
    private bool isDoneClostering;
    // Start is called before the first frame update
    void Start()
    {
        points = new List<GameObject>();
        centroids = new List<GameObject>();
        clusteredPoints = new Dictionary<GameObject, List<GameObject>>();
        StartKMeansClustering();
    }

    private void Cluster()
    {
        // Reset clusterData
        clusteredPoints.Clear();

        // Populate centroids
        for (int i = 0; i < Centroids; i++)
        {
            clusteredPoints.Add(centroids[i], new List<GameObject>());
        }

        // Compute distance from each point to each centroid
        for (int i = 0; i < Points; i++)
        {
            var pointPosition = points[i].transform.position;
            var minDistance = float.MaxValue;
            GameObject closestCentroid = centroids[0];
            for (int j = 0; j < Centroids; j++)
            {
                var distance = Mathf.Abs(Vector3.Distance(centroids[j].transform.position, pointPosition));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCentroid = centroids[j];
                }
            }
            clusteredPoints[closestCentroid].Add(points[i]);
        }

        // Set colors
        int clusterCounter = 0;
        foreach (var cluster in clusteredPoints)
        {
            foreach (var point in cluster.Value)
            {
                point.GetComponent<MeshRenderer>().material.color = colors[clusterCounter];
            }
            clusterCounter++;
        }

        // Recompute positions for centroids

    }

    public void StartKMeansClustering()
    {
        ClearData();
        points = GenerateGameObjects(Point, Points);
        centroids = GenerateGameObjects(Centroid, Centroids);
        colors = GenerateColors();

        // Set random colors for centroids and add centroids to dictionary
        for (int i = 0; i < Centroids; i++)
        {
            centroids[i].GetComponent<MeshRenderer>().material.color = colors[i];
        }

        Cluster();


    }

    private List<Color> GenerateColors()
    {
        List<Color> result = new List<Color>();
        for (int i = 0; i < Centroids; i++)
        {
            result.Add(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }
        return result;
    }

    private void ClearData()
    {
        DeleteChildren();
        points.Clear();
        centroids.Clear();
        clusteredPoints.Clear();
        isDoneClostering = false;
    }

    private void DeleteChildren()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }

    private List<GameObject> GenerateGameObjects(GameObject prefab, int size)
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < size; i++)
        {
            var positionX = UnityEngine.Random.Range(-Width / 2, Width / 2);
            var positionZ = UnityEngine.Random.Range(-Height / 2, Height / 2);
            var newGameObject = Instantiate(prefab, 
                new Vector3(positionX, prefab.transform.position.y, positionZ),
                Quaternion.identity, transform
            );
            result.Add(newGameObject);
        }

        return result;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
