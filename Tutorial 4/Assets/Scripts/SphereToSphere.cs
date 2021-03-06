﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereToSphere : MonoBehaviour
{
    public Transform TestPoint;
    public Material CollisionMaterial;
    private Material defaultMaterial;
    private Renderer testPointRenderer;
    private SphereCollider sphereCollider;
    private SphereCollider testPointSphereCollider;

    private void Start()
    {
        testPointRenderer = TestPoint.GetComponent<Renderer>();
        sphereCollider = GetComponent<SphereCollider>();
        testPointSphereCollider = TestPoint.GetComponent<SphereCollider>();
        defaultMaterial = testPointRenderer.material;
    }

    private void Update()
    {
        //Don't do it this way (commented out code) as square root operation is inefficient
        //Instead compare the squared distance against the squared radius
        //var distance = Mathf.Sqrt((TestPoint.position.x - transform.position.x) * (TestPoint.position.x - transform.position.x) +
        //               (TestPoint.position.y - transform.position.y) * (TestPoint.position.y - transform.position.y) +
        //               (TestPoint.position.z - transform.position.z) * (TestPoint.position.z - transform.position.z));

        var distance = (TestPoint.position.x - transform.position.x) * (TestPoint.position.x - transform.position.x) +
                       (TestPoint.position.y - transform.position.y) * (TestPoint.position.y - transform.position.y) +
                       (TestPoint.position.z - transform.position.z) * (TestPoint.position.z - transform.position.z);

        var radiusSqaure = (sphereCollider.radius + testPointSphereCollider.radius) * (sphereCollider.radius + testPointSphereCollider.radius);
        if (distance < radiusSqaure)
        {
            Debug.Log("Collision!");
            testPointRenderer.material = CollisionMaterial;
        }
        else
        {
            testPointRenderer.material = defaultMaterial;
        }
    }
}
