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
    public Transform PointsHolder;
    public Transform CentroidsHolder;
    public GameObject DoneText;
    private List<GameObject> points;
    private List<GameObject> centroids;
    private List<Color> colors;
    private Dictionary<GameObject, List<GameObject>> clusteredPoints;
    private List<GameObject> previousCentroids;
    // Start is called before the first frame update
    void Start()
    {
        points = new List<GameObject>();
        centroids = new List<GameObject>();
        previousCentroids = new List<GameObject>();
        clusteredPoints = new Dictionary<GameObject, List<GameObject>>();
        StartKMeansClustering();
    }

    public void Cluster()
    {
        // Reset clusterData
        clusteredPoints.Clear();

        // Populate cluster dictionary
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

        // Check if there is a cluster with no elements
        foreach (var cluster in clusteredPoints)
        {
            if (cluster.Value.Count == 0)
            {
                var closestPoint = GetClosestPoint(cluster.Key.transform.position);
                RemovePointFromClusters(closestPoint);
                cluster.Value.Add(closestPoint);
            }
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
        clusterCounter = 0;
        foreach (var cluster in clusteredPoints)
        {
            Vector3 sum = Vector3.zero;
            foreach (var point in cluster.Value)
            {
                sum += point.transform.position;
            }
            var average = sum / cluster.Value.Count;
            centroids[clusterCounter].transform.position = average;

            clusterCounter++;
        }

        // Check if no centroids changed their position
        CheckForEnd();

        // Update previous centroids to current positions
        UpdatePreviousCentroids();
    }

    private void CheckForEnd()
    {
        int centroidsChanged = 0;
        for (int i = 0; i < centroids.Count; i++)
        {
            if (centroids[i].transform.position != previousCentroids[i].transform.position)
            {
                centroidsChanged++;
            }
        }
        if (centroidsChanged == 0)
        {
            DoneText.SetActive(true);
        }
    }

    private void UpdatePreviousCentroids()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            previousCentroids[i].transform.position = centroids[i].transform.position;
        }
    }

    private void RemovePointFromClusters(GameObject closestPoint)
    {
        GameObject itemToBeRemoved = null;
        GameObject removeFromCluster = null;
        foreach (var item in clusteredPoints)
        {
            foreach (var point in item.Value)
            {
                if (point == closestPoint)
                {
                    itemToBeRemoved = point;
                    removeFromCluster = item.Key;
                }
            }
        }

        if (removeFromCluster)
        {
            clusteredPoints[removeFromCluster].Remove(itemToBeRemoved);
        }
    }

    private GameObject GetClosestPoint(Vector3 position)
    {
        var closestPoint = points[0];
        var shortestDistance = float.MaxValue;

        foreach (var item in clusteredPoints)
        {
            foreach (var point in item.Value)
            {
                var currentDistance = Vector3.Distance(point.transform.position, position);
                if (currentDistance < shortestDistance && item.Value.Count > 1)
                {
                    closestPoint = point;
                    shortestDistance = currentDistance;
                }
            }
        }
        return closestPoint;
    }

    public void StartKMeansClustering()
    {
        ClearData();
        points = GenerateGameObjects(Point, Points, PointsHolder);
        centroids = GenerateGameObjects(Centroid, Centroids, CentroidsHolder);
        colors = GenerateColors();

        // Set random colors for centroids and add centroids to dictionary
        for (int i = 0; i < Centroids; i++)
        {
            centroids[i].GetComponent<MeshRenderer>().material.color = colors[i];
        }

        // Update previousCentroids to current centroids
        // Create new GameObjects instead of duplicating list elements (would require deep copy)
        foreach (var item in centroids)
        {
            previousCentroids.Add(new GameObject());
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
        DeleteChildren(PointsHolder);
        DeleteChildren(CentroidsHolder);
        points.Clear();
        centroids.Clear();
        clusteredPoints.Clear();
        DoneText.SetActive(false);
    }

    private void DeleteChildren(Transform parent)
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }

    private List<GameObject> GenerateGameObjects(GameObject prefab, int size, Transform parent)
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < size; i++)
        {
            var positionX = UnityEngine.Random.Range(-Width / 2 + prefab.transform.localScale.x,
                Width / 2 - prefab.transform.localScale.x);
            var positionZ = UnityEngine.Random.Range(-Height / 2 + prefab.transform.localScale.z,
                Height / 2 - prefab.transform.localScale.z);
            var newGameObject = Instantiate(prefab,
                new Vector3(positionX, prefab.transform.position.y, positionZ),
                Quaternion.identity, parent);
            result.Add(newGameObject);
        }

        return result;
    }
}
